# Contact Management System - Deployment Guide

## Overview
This guide explains how to deploy the Contact Management System as a standalone executable and create an installer.

---

## Option 1: Self-Contained Executable (Recommended)

### What is Self-Contained Executable?
- Single EXE file that includes .NET runtime
- Users don't need to install .NET separately
- Larger file size (~100MB) but easier installation
- Works on Windows 10/11+

### Steps to Create Self-Contained EXE:

1. **Open PowerShell in the project directory:**
   ```powershell
   cd e:\Contact_Management_System\ContactManagementAPI
   ```

2. **Publish as self-contained executable:**
   ```powershell
   dotnet publish -c Release -r win-x64 --self-contained
   ```

3. **Find the output:**
   - Location: `ContactManagementAPI/bin/Release/net8.0/win-x64/publish/`
   - Main EXE: `ContactManagementAPI.exe`

4. **Copy the entire `publish` folder to distribution**
   - All files in the publish folder are required
   - Create a shortcut to `ContactManagementAPI.exe`

---

## Option 2: Framework-Dependent Executable

### What is Framework-Dependent Executable?
- Smaller file size (~10MB)
- Requires .NET 8 runtime to be installed on user's machine
- Easier to update

### Steps to Create:

1. **Publish as framework-dependent:**
   ```powershell
   cd e:\Contact_Management_System\ContactManagementAPI
   dotnet publish -c Release -r win-x64 --no-self-contained
   ```

2. **Create EXE wrapper** (optional)
   - Use a batch file to run the application

---

## Option 3: Run with Batch File (Simplest)

### Create a Quick-Start Batch File:

Create file: `Run_ContactManagementSystem.bat`

```batch
@echo off
cd /d "%~dp0ContactManagementAPI"
dotnet run --configuration Release
pause
```

**Advantages:**
- No additional setup required
- Works immediately
- Users just double-click the .bat file

---

## Option 4: Create a Setup Installer

### Using Inno Setup (Free & Recommended)

#### Step 1: Download Inno Setup
- Download from: https://jrsoftware.org/isdl.php
- Install it

#### Step 2: Create Setup Script

Create file: `ContactManagementSystem-Setup.iss`

```ini
; Inno Setup Script for Contact Management System
[Setup]
AppName=Contact Management System
AppVersion=1.0.0
AppPublisher=Your Organization
AppPublisherURL=https://github.com/abraham9486937737/Contact-Management-System
DefaultDirName={autopf}\Contact Management System
DefaultGroupName=Contact Management System
OutputBaseFilename=ContactManagementSystem-Setup-1.0.0
Compression=lzma
SolidCompression=yes
PrivilegesRequired=lowest
DisableDirPage=no
DisableProgramGroupPage=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "ContactManagementAPI\bin\Release\net8.0\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Contact Management System"; Filename: "{app}\ContactManagementAPI.exe"; WorkingDir: "{app}"
Name: "{group}\Uninstall Contact Management System"; Filename: "{uninstallexe}"
Name: "{autodesktop}\Contact Management System"; Filename: "{app}\ContactManagementAPI.exe"; WorkingDir: "{app}"

[Run]
Filename: "{app}\ContactManagementAPI.exe"; Description: "Launch Contact Management System"; Flags: nowait postinstall skipifsilent WorkingDir: "{app}"

[InstallDelete]
Type: filesandordirs; Name: "{app}\*"
```

#### Step 3: Build Setup File
1. Open Inno Setup Compiler
2. File → Open → Select `ContactManagementSystem-Setup.iss`
3. Click "Compile"
4. Setup file will be created in the same directory

---

## Recommended Deployment Approach

### For End Users:
1. **Best Option**: Use Self-Contained EXE (Option 1)
   - Create shortcut on desktop
   - Users just click to run
   - No dependencies to install

2. **Create Desktop Shortcut:**
   - Right-click `ContactManagementAPI.exe`
   - Send to → Desktop (create shortcut)
   - Rename to "Contact Management System"
   - Right-click → Properties → Change Icon (optional)

### For Deployment Team:
1. Publish as self-contained executable
2. Create installer with Inno Setup
3. Distribute MSI/EXE file
4. Users install and run

---

## Database Setup for Users

**Important:** Before running the application:

1. **Database Creation:**
   - Application automatically creates SQLite database
   - Or configure SQL Server connection string in `appsettings.json`

2. **Migrations:**
   - First run automatically applies migrations
   - No manual database setup needed

---

## Best Way to Use This Application

### Development Environment:
```powershell
cd ContactManagementAPI
dotnet run
# Application runs at http://localhost:5000
```

### Production Deployment:

**Step 1: Create Self-Contained EXE**
```powershell
cd ContactManagementAPI
dotnet publish -c Release -r win-x64 --self-contained
```

**Step 2: Create Installer (Optional)**
- Use Inno Setup script provided
- Distribute `.exe` installer file

**Step 3: User Installation**
- Double-click installer
- Select installation directory
- Finish installation
- Click shortcut to launch

---

## System Requirements

### Minimum Requirements:
- **OS**: Windows 10 (Build 1909+) or Windows 11
- **RAM**: 2GB
- **Storage**: 500MB free space
- **Display**: 1024x768 minimum

### With Self-Contained EXE:
- No additional software needed
- .NET runtime included

### With Framework-Dependent EXE:
- .NET 8 Runtime must be installed
- Download from: https://dotnet.microsoft.com/en-us/download/dotnet/8.0

---

## Troubleshooting

### Application won't start:
1. Check Windows Firewall - allow port 5000
2. Ensure database file has write permissions
3. Check `appsettings.json` for correct connection string

### Database issues:
1. Delete existing database
2. Run application - it will recreate database
3. Sample data loads automatically on first run

### Port 5000 in use:
- Edit `Properties/launchSettings.json`
- Change port to 5001 or 5002

---

## Performance Tips

1. **Use SQL Server** instead of SQLite for better performance
2. **Enable caching** in production
3. **Use HTTPS** for security
4. **Monitor logs** for issues
5. **Regular backups** of database

---

## Security Considerations

1. **Change database credentials** in production
2. **Enable HTTPS** 
3. **Use strong passwords** for database
4. **Restrict file uploads** size and types
5. **Regular security updates**

---

## Support & Updates

- **GitHub**: https://github.com/abraham9486937737/Contact-Management-System
- **Report Issues**: GitHub Issues
- **Pull Requests**: Welcome for improvements

---

## Summary

**Easiest Way for Users:**
1. Download published EXE folder
2. Create desktop shortcut
3. Double-click shortcut to run

**Best Way for Organization:**
1. Create installer with Inno Setup
2. Deploy to users via installer
3. Automatic updates via new installer releases

**For Development:**
1. Clone repository
2. Run `dotnet run` in ContactManagementAPI folder
3. Access at http://localhost:5000
