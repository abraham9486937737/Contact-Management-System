param(
    [string]$SqlServerInstance = ".\SQLEXPRESS",
    [string]$InstallDir = "$env:LOCALAPPDATA\ContactManagementSystem",
    [ValidateSet("Auto", "Published", "Build")]
    [string]$SourceMode = "Auto",
    [switch]$NoDesktopShortcut,
    [switch]$NoLaunch
)

$ErrorActionPreference = "Stop"

function Write-Header {
    param([string]$Message)
    Write-Host ""
    Write-Host "============================================================" -ForegroundColor Cyan
    Write-Host " $Message" -ForegroundColor Cyan
    Write-Host "============================================================" -ForegroundColor Cyan
    Write-Host ""
}

function Write-Info {
    param([string]$Message)
    Write-Host "[INFO] $Message" -ForegroundColor Gray
}

function Write-Success {
    param([string]$Message)
    Write-Host "[OK]   $Message" -ForegroundColor Green
}

function Write-ErrorMessage {
    param([string]$Message)
    Write-Host "[ERR]  $Message" -ForegroundColor Red
}

$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectDir = Join-Path $repoRoot "ContactManagementAPI"
$projectFile = Join-Path $projectDir "ContactManagementAPI.csproj"
$publishedDir = Join-Path $repoRoot "Published"
$temporaryPublishDir = Join-Path $repoRoot ".local_publish"

if (-not (Test-Path $projectFile)) {
    Write-ErrorMessage "Could not find ContactManagementAPI.csproj. Run this script from the repository root."
    exit 1
}

Write-Header "Contact Management System - Local Installer"
Write-Info "SQL Server instance: $SqlServerInstance"
Write-Info "Install directory: $InstallDir"
Write-Info "Source mode: $SourceMode"

$sourceDir = $null
$publishedExe = Join-Path $publishedDir "ContactManagementAPI.exe"

$shouldUsePublished = $false
if (($SourceMode -eq "Auto" -or $SourceMode -eq "Published") -and (Test-Path $publishedExe)) {
    if ($SourceMode -eq "Published") {
        $shouldUsePublished = $true
    }
    else {
        $publishedDll = Join-Path $publishedDir "ContactManagementAPI.dll"
        $publishedTime = if (Test-Path $publishedDll) { (Get-Item $publishedDll).LastWriteTimeUtc } else { (Get-Item $publishedExe).LastWriteTimeUtc }

        $latestSourceTime = Get-ChildItem -Path $projectDir -Recurse -File |
            Where-Object { $_.FullName -notmatch "\\bin\\|\\obj\\" } |
            Sort-Object LastWriteTimeUtc -Descending |
            Select-Object -First 1 -ExpandProperty LastWriteTimeUtc

        if ($publishedTime -ge $latestSourceTime) {
            $shouldUsePublished = $true
            Write-Info "Published package is up-to-date with source"
        }
        else {
            Write-Info "Published package is older than source; switching to fresh build"
        }
    }
}

if ($shouldUsePublished) {
    $sourceDir = $publishedDir
    Write-Success "Using pre-published application package"
}

if (-not $sourceDir) {
    if ($SourceMode -eq "Published") {
        Write-ErrorMessage "Published package not found in 'Published'. Use SourceMode Build or Auto."
        exit 1
    }

    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
        Write-ErrorMessage "dotnet SDK is required for build mode. Install .NET 8 SDK and run again."
        Write-Host "Download: https://dotnet.microsoft.com/download/dotnet/8.0"
        exit 1
    }

    Write-Info "Building self-contained app package for win-x64..."
    if (Test-Path $temporaryPublishDir) {
        Remove-Item -Path $temporaryPublishDir -Recurse -Force
    }

    Push-Location $projectDir
    try {
        dotnet restore
        if ($LASTEXITCODE -ne 0) {
            throw "dotnet restore failed"
        }

        dotnet publish -c Release -r win-x64 --self-contained -o $temporaryPublishDir
        if ($LASTEXITCODE -ne 0) {
            throw "dotnet publish failed"
        }
    }
    finally {
        Pop-Location
    }

    $sourceDir = $temporaryPublishDir
    Write-Success "Build completed"
}

$sourceAppSettings = Join-Path $sourceDir "appsettings.json"
if (-not (Test-Path $sourceAppSettings)) {
    Write-ErrorMessage "appsettings.json not found in source package."
    exit 1
}

if (Test-Path $InstallDir) {
    Write-Info "Cleaning existing installation..."
    Remove-Item -Path $InstallDir -Recurse -Force
}
New-Item -Path $InstallDir -ItemType Directory -Force | Out-Null
Copy-Item -Path (Join-Path $sourceDir "*") -Destination $InstallDir -Recurse -Force
Write-Success "Installed files copied"

$installedAppSettings = Join-Path $InstallDir "appsettings.json"
$connectionString = "Server=$SqlServerInstance;Database=ContactManagementDB;Trusted_Connection=true;TrustServerCertificate=true;Encrypt=false;"
$json = Get-Content -Path $installedAppSettings -Raw | ConvertFrom-Json
if (-not $json.ConnectionStrings) {
    $json | Add-Member -MemberType NoteProperty -Name "ConnectionStrings" -Value @{}
}
$json.ConnectionStrings.DefaultConnection = $connectionString
$json | ConvertTo-Json -Depth 20 | Set-Content -Path $installedAppSettings -Encoding UTF8
Write-Success "Updated connection string in installed app"

$runBatPath = Join-Path $InstallDir "Run-ContactManagementSystem.bat"
$runBatContent = @"
@echo off
title Contact Management System
echo.
echo ========================================
echo  Contact Management System
echo ========================================
echo.
echo Starting application at http://localhost:5000
echo Press Ctrl+C to stop the application
echo.
cd /d "%~dp0"
start "" "http://localhost:5000"
ContactManagementAPI.exe
"@
$runBatContent | Set-Content -Path $runBatPath -Encoding ASCII
Write-Success "Created launcher: Run-ContactManagementSystem.bat"

if (-not $NoDesktopShortcut) {
    $desktopPath = [Environment]::GetFolderPath("Desktop")
    $shortcutPath = Join-Path $desktopPath "Contact Management System.lnk"
    $targetPath = $runBatPath
    $iconPath = Join-Path $InstallDir "ContactManagementAPI.exe"

    $wshShell = New-Object -ComObject WScript.Shell
    $shortcut = $wshShell.CreateShortcut($shortcutPath)
    $shortcut.TargetPath = $targetPath
    $shortcut.WorkingDirectory = $InstallDir
    if (Test-Path $iconPath) {
        $shortcut.IconLocation = $iconPath
    }
    $shortcut.Save()

    Write-Success "Created desktop shortcut"
}

if (-not $NoLaunch) {
    Start-Process -FilePath $runBatPath
    Write-Success "Application launched"
}

if (Test-Path $temporaryPublishDir) {
    Remove-Item -Path $temporaryPublishDir -Recurse -Force -ErrorAction SilentlyContinue
}

Write-Host ""
Write-Host "Installation complete." -ForegroundColor Green
Write-Host "Install location: $InstallDir"
Write-Host "Run anytime using: $runBatPath"