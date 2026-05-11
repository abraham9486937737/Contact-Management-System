# Contact Management System - Day-to-Day Use Guide

This guide is for daily operation of the app hosted in IIS on your PC.

## 1) Main URL

- Local URL: http://localhost:8080
- Same-network phone/laptop URL: http://<your-pc-ip>:8080

Note: The app works when your PC is ON and IIS service is running.

## 2) Login Credentials

Use these seeded accounts (based on current code):

- Username: `admin`
- Password: `Admin@123`

Super Admin:

- Username: `abrahamcbe@gmail.com`
- Password: `M@ld1ves`

## 3) Daily Use (No Rebuild Needed)

Normally, just open the URL:

```powershell
http://localhost:8080
```

If site is stopped, start it:

```powershell
Start-Website -Name "ContactManagementSystem"
```

If needed, stop it:

```powershell
Stop-Website -Name "ContactManagementSystem"
```

## 4) Redeploy After Code Changes

Run this from Admin PowerShell:

```powershell
Set-Location E:\Contact_Management_System
.\Setup-IIS-Hosting.ps1 -SiteName "ContactManagementSystem" -AppPoolName "ContactManagementSystemPool" -Port 8080 -ForceRecreateSite
```

This command does all required steps:

- Stops running app process (if any)
- Publishes latest code to `Published\IIS`
- Recreates IIS site/app pool mapping
- Applies folder permissions
- Tests URL

## 5) Quick Health Check

```powershell
Invoke-WebRequest http://localhost:8080 -UseBasicParsing
```

Expected: `StatusCode : 200`

## 6) One-Time Setup Commands (Already Completed)

Only needed on new machines:

```powershell
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole,IIS-WebServer,IIS-ManagementConsole,IIS-ManagementScriptingTools -All
winget install Microsoft.DotNet.HostingBundle.8
iisreset
```

## 7) Common Issues and Fast Fixes

### A) Port not responding

```powershell
Start-Website -Name "ContactManagementSystem"
Invoke-WebRequest http://localhost:8080 -UseBasicParsing
```

### B) Build/publish says file is locked

```powershell
Stop-Process -Name ContactManagementAPI -Force
```

Then re-run redeploy command.

### C) HTTP 500/500.19 in IIS

```powershell
iisreset
```

Then redeploy again.

### D) Missing old seeded photo files (404 for uploads)

This is usually non-blocking. Re-upload photo from Edit Contact page if needed.

## 8) Useful File Paths

- IIS deployment output: `E:\Contact_Management_System\Published\IIS`
- IIS setup/redeploy script: `E:\Contact_Management_System\Setup-IIS-Hosting.ps1`
- Project source: `E:\Contact_Management_System\ContactManagementAPI`

## 9) Recommended Routine

1. Open `http://localhost:8080`
2. Login and use app
3. If code changed, run the redeploy command once
4. Run health check if any issue appears
