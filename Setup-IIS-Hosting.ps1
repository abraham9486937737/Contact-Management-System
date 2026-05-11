param(
    [string]$SiteName = "ContactManagementSystem",
    [string]$AppPoolName = "ContactManagementSystemPool",
    [int]$Port = 8080,
    [string]$PublishPath = "",
    [switch]$ForceRecreateSite,
    [switch]$SkipPublish
)

$ErrorActionPreference = "Stop"

function Write-Info {
    param([string]$Message)
    Write-Host "[INFO] $Message" -ForegroundColor Cyan
}

function Write-Ok {
    param([string]$Message)
    Write-Host "[OK]   $Message" -ForegroundColor Green
}

function Write-Warn {
    param([string]$Message)
    Write-Host "[WARN] $Message" -ForegroundColor Yellow
}

function Write-Err {
    param([string]$Message)
    Write-Host "[ERR]  $Message" -ForegroundColor Red
}

function Require-Administrator {
    $currentUser = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = New-Object Security.Principal.WindowsPrincipal($currentUser)
    if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
        throw "Please run this script as Administrator."
    }
}

function Import-IisModuleOrThrow {
    try {
        Import-Module WebAdministration -ErrorAction Stop
        return
    }
    catch {
        $hint = @"
IIS PowerShell module is missing.

Run this in an Administrator PowerShell window:
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole,IIS-WebServer,IIS-ManagementConsole,IIS-ManagementScriptingTools -All

Then close and reopen PowerShell and run this setup script again.
"@
        throw $hint
    }
}

function Stop-RunningAppProcesses {
    $processes = Get-Process -Name "ContactManagementAPI" -ErrorAction SilentlyContinue
    if ($processes) {
        Write-Warn "Stopping running ContactManagementAPI process(es) before publish..."
        foreach ($p in $processes) {
            try {
                Stop-Process -Id $p.Id -Force -ErrorAction Stop
                Write-Info "Stopped ContactManagementAPI process ID $($p.Id)"
            }
            catch {
                Write-Warn "Could not stop process ID $($p.Id): $($_.Exception.Message)"
            }
        }
    }
}

function Ensure-AspNetCoreIisModule {
    $ancmDll = "C:\Program Files\IIS\Asp.Net Core Module\V2\aspnetcorev2.dll"
    if (-not (Test-Path $ancmDll)) {
        $hint = @"
ASP.NET Core Module V2 was not found.

Install the .NET 8 ASP.NET Core Hosting Bundle, then restart IIS:
1) Download: https://dotnet.microsoft.com/en-us/download/dotnet/8.0
2) Install "ASP.NET Core Runtime 8 Hosting Bundle" for Windows.
3) Run: iisreset

After that, re-run this script.
"@
        throw $hint
    }
}

try {
    Require-Administrator

    $repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
    $projectFile = Join-Path $repoRoot "ContactManagementAPI\ContactManagementAPI.csproj"

    if (-not (Test-Path $projectFile)) {
        throw "Could not find ContactManagementAPI.csproj. Run this script from the repository root."
    }

    if (-not $PublishPath) {
        $PublishPath = Join-Path $repoRoot "Published\IIS"
    }

    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
        throw "dotnet SDK/runtime was not found. Install .NET 8 SDK first."
    }

    Import-IisModuleOrThrow
    Ensure-AspNetCoreIisModule

    if (-not $SkipPublish) {
        Stop-RunningAppProcesses

        Write-Info "Publishing application for IIS..."

        if (Test-Path $PublishPath) {
            Remove-Item -Path $PublishPath -Recurse -Force
        }

        New-Item -Path $PublishPath -ItemType Directory -Force | Out-Null

        dotnet publish $projectFile -c Release --no-self-contained -o $PublishPath
        if ($LASTEXITCODE -ne 0) {
            throw "dotnet publish failed."
        }

        Write-Ok "Publish completed: $PublishPath"
    }

    if (-not (Test-Path $PublishPath)) {
        throw "Publish path does not exist: $PublishPath"
    }

    $webConfig = Join-Path $PublishPath "web.config"
    if (-not (Test-Path $webConfig)) {
        Write-Warn "web.config not found in publish output. Ensure ASP.NET Core Hosting Bundle is installed on this PC."
    }

    $appPoolPath = "IIS:\AppPools\$AppPoolName"
    if (-not (Test-Path $appPoolPath)) {
        New-WebAppPool -Name $AppPoolName | Out-Null
        Write-Ok "Created app pool: $AppPoolName"
    }
    else {
        Write-Info "App pool already exists: $AppPoolName"
    }

    Set-ItemProperty $appPoolPath -Name "managedRuntimeVersion" -Value ""
    Set-ItemProperty $appPoolPath -Name "managedPipelineMode" -Value "Integrated"
    Set-ItemProperty $appPoolPath -Name "processModel.identityType" -Value 4

    $sitePath = "IIS:\Sites\$SiteName"
    if (Test-Path $sitePath) {
        if ($ForceRecreateSite) {
            Remove-Website -Name $SiteName
            Write-Warn "Removed existing site: $SiteName"
        }
        else {
            throw "Site '$SiteName' already exists. Re-run with -ForceRecreateSite to recreate it."
        }
    }

    New-Website -Name $SiteName -Port $Port -PhysicalPath $PublishPath -ApplicationPool $AppPoolName | Out-Null
    Write-Ok "Created IIS site: $SiteName on port $Port"

    $appPoolIdentity = "IIS AppPool\$AppPoolName"
    & icacls $PublishPath /grant "$($appPoolIdentity):(OI)(CI)(M)" /T | Out-Null
    Write-Ok "Granted folder permissions to app pool identity"

    Start-WebAppPool -Name $AppPoolName
    Start-Website -Name $SiteName

    $url = "http://localhost:$Port"
    Write-Info "Testing URL: $url"

    try {
        $response = Invoke-WebRequest -Uri $url -UseBasicParsing -TimeoutSec 20
        Write-Ok "Application is responding (HTTP $($response.StatusCode))"
    }
    catch {
        Write-Warn "Site created, but test request did not return success yet. Open IIS Manager and check site logs/event viewer."
        Write-Warn $_.Exception.Message
    }

    Write-Host ""
    Write-Ok "IIS hosting setup complete"
    Write-Host "Open: $url"
    Write-Host "To stop: Stop-Website -Name \"$SiteName\""
    Write-Host "To start: Start-Website -Name \"$SiteName\""
}
catch {
    Write-Err $_.Exception.Message
    exit 1
}
