# ========================================
# Contact Management System - Auto Publisher
# ========================================
# This script automates the creation of deployable executables

param(
    [ValidateSet("self-contained", "framework-dependent", "both")]
    [string]$PublishType = "self-contained",
    
    [ValidateSet("win-x64", "win-x86", "win-arm64")]
    [string]$RuntimeIdentifier = "win-x64"
)

$ErrorActionPreference = "Stop"

# Colors for output
function Write-Header {
    param([string]$Message)
    Write-Host ""
    Write-Host "============================================================" -ForegroundColor Cyan
    Write-Host "$Message" -ForegroundColor Cyan
    Write-Host "============================================================" -ForegroundColor Cyan
    Write-Host ""
}

function Write-Success {
    param([string]$Message)
    Write-Host "[OK] $Message" -ForegroundColor Green
}

function Write-Info {
    param([string]$Message)
    Write-Host "[INFO] $Message" -ForegroundColor Cyan
}

function Write-Warning {
    param([string]$Message)
    Write-Host "[WARN] $Message" -ForegroundColor Yellow
}

function Write-Error {
    param([string]$Message)
    Write-Host "[ERR] $Message" -ForegroundColor Red
}

# Main script
Write-Header "Contact Management System - Publisher"

# Check if in correct directory
if (-not (Test-Path "ContactManagementAPI/ContactManagementAPI.csproj")) {
    Write-Error "ContactManagementAPI project not found in current directory!"
    Write-Info "Please run this script from the root of the Contact Management System repository"
    exit 1
}

Write-Info "Starting publish process..."
Write-Info "Publish Type: $PublishType"
Write-Info "Runtime: $RuntimeIdentifier"

try {
    # Clean previous builds
    Write-Info "Cleaning previous builds..."
    Set-Location ContactManagementAPI
    Remove-Item -Path "bin\Release" -Recurse -Force -ErrorAction SilentlyContinue | Out-Null
    Write-Success "Cleaned previous builds"

    # Self-contained executable
    if ($PublishType -eq "self-contained" -or $PublishType -eq "both") {
        Write-Header "Publishing Self-Contained Executable"
        Write-Info "This includes .NET runtime (~100MB)"
        
        dotnet publish -c Release -r $RuntimeIdentifier --self-contained
        
        if ($LASTEXITCODE -eq 0) {
            Write-Success "Self-contained executable published successfully!"
            Write-Info "Location: bin/Release/net8.0/$RuntimeIdentifier/publish/"
            Write-Info "Size: ~100-150MB"
            Write-Info "Users: No .NET installation required"
        } else {
            Write-Error "Failed to publish self-contained executable"
            exit 1
        }
    }

    # Framework-dependent executable
    if ($PublishType -eq "framework-dependent" -or $PublishType -eq "both") {
        Write-Header "Publishing Framework-Dependent Executable"
        Write-Info "This requires .NET 8 runtime on target machine (~10MB)"
        
        Remove-Item -Path "bin\Release" -Recurse -Force -ErrorAction SilentlyContinue | Out-Null
        
        dotnet publish -c Release -r $RuntimeIdentifier --no-self-contained
        
        if ($LASTEXITCODE -eq 0) {
            Write-Success "Framework-dependent executable published successfully!"
            Write-Info "Location: bin/Release/net8.0/$RuntimeIdentifier/publish/"
            Write-Info "Size: ~10-15MB"
            Write-Info "Users: Must have .NET 8 runtime installed"
            Write-Info "Download .NET from: https://dotnet.microsoft.com/download/dotnet/8.0"
        } else {
            Write-Error "Failed to publish framework-dependent executable"
            exit 1
        }
    }

    # Create deployment shortcuts
    Write-Header "Creating Deployment Shortcuts"
    
    $publishPath = "bin\Release\net8.0\$RuntimeIdentifier\publish"
    
    # Verify publish folder exists
    if (Test-Path $publishPath) {
        $exePath = Join-Path $publishPath "ContactManagementAPI.exe"
        
        if (Test-Path $exePath) {
            Write-Success "Executable found: $exePath"
            
            # Create a batch file to run the app
            $batchContent = @"
@echo off
cd /d "%~dp0"
ContactManagementAPI.exe
pause
"@
            
            $batchPath = Join-Path $publishPath "Run.bat"
            $batchContent | Out-File -FilePath $batchPath -Encoding ASCII
            Write-Success "Created Run.bat shortcut"
            
            # Create ReadMe for deployment
            $readmeContent = @"
# Contact Management System - Deployment Package

## Quick Start
1. Double-click `Run.bat` to start the application
2. Application opens at http://localhost:5000
3. Press Ctrl+C to stop

## Create Desktop Shortcut (Optional)
1. Right-click `ContactManagementAPI.exe`
2. Select "Send to" > "Desktop (create shortcut)"
3. Rename shortcut to "Contact Management System"
4. Double-click to run

## System Requirements
- Windows 10 (Build 1909+) or Windows 11
- 2GB RAM
- 500MB free disk space

## Troubleshooting
- If port 5000 is in use, edit appsettings.json
- Check Windows Firewall if connection fails
- Ensure folder has write permissions for database

## Support
GitHub: https://github.com/abraham9486937737/Contact-Management-System
"@
            
            $readmePath = Join-Path $publishPath "README.txt"
            $readmeContent | Out-File -FilePath $readmePath -Encoding UTF8
            Write-Success "Created README.txt"
        }
    }

    Set-Location ..

    Write-Header "Publication Complete!"
    Write-Success "Your application is ready for deployment"
    Write-Info "Publish location: ContactManagementAPI\bin\Release\net8.0\$RuntimeIdentifier\publish\"
    Write-Info ""
    Write-Info "Next steps:"
    Write-Info "1. Copy the entire publish folder to distribution"
    Write-Info "2. Users can extract and run by double-clicking Run.bat"
    Write-Info "3. Or copy and run the EXE directly"
    Write-Info ""
    Write-Info "Optional: Create installer with Inno Setup"
    Write-Info "See DEPLOYMENT_GUIDE.md for details"
    
} catch {
    Write-Error "An error occurred: $_"
    exit 1
}

Write-Host ""
