# Contact Management System - Quick Start Guide

## 🚀 Easiest Way to Run (Recommended)

### Option 1: Using Batch File (Simplest)
```
1. Navigate to: e:\Contact_Management_System\
2. Double-click: Run_ContactManagementSystem.bat
3. Wait for: "Application will be available at: http://localhost:5000"
4. Open browser: http://localhost:5000
5. Start using the application!
```

**That's it!** No additional setup needed.

---

## 📦 How to Create EXE & Distribute

### Step 1: Publish as Self-Contained Executable (One-time)

**Using PowerShell Script (Recommended):**
```powershell
cd e:\Contact_Management_System
.\Publish-Application.ps1 -PublishType self-contained
```

**Or manually:**
```powershell
cd e:\Contact_Management_System\ContactManagementAPI
dotnet publish -c Release -r win-x64 --self-contained
```

**Output Location:**
```
ContactManagementAPI\bin\Release\net8.0\win-x64\publish\
```

### Step 2: What You Get
- `ContactManagementAPI.exe` - Main application
- `Run.bat` - Easy launcher script
- `README.txt` - Instructions for users
- All supporting files (~100-150 MB total)

### Step 3: Distribute to Users

**Option A: Direct Folder**
```
1. Copy entire 'publish' folder
2. Zip it: ContactManagementSystem-v1.0.0.zip
3. Share with users
4. Users extract and double-click Run.bat
```

**Option B: Create Professional Installer**
```
1. Download Inno Setup: https://jrsoftware.org/isdl.php
2. Open: ContactManagementSystem-Setup.iss
3. Click: Build → Compile
4. Share: Installers\ContactManagementSystem-Installer-1.0.0.exe
5. Users run installer, click next, done!
```

---

## 📋 System Requirements for Users

| Requirement | Detail |
|---|---|
| **OS** | Windows 10 (Build 1909+) or Windows 11 |
| **Processor** | 64-bit processor |
| **RAM** | 2 GB minimum |
| **Storage** | 500 MB free space |
| **Display** | 1024x768 or higher |
| **.NET Runtime** | Included in self-contained version |

---

## 🎯 Best Deployment Method

### For Small Teams (1-10 Users):
```
1. Publish as self-contained
2. Create Run_ContactManagementSystem.bat shortcut
3. Email users the publish folder (zip it)
4. Users extract and run batch file
```

### For Larger Organizations:
```
1. Create installer with Inno Setup
2. Distribute installer via network/email
3. Users install and get Start Menu shortcuts
4. Single-click to launch
```

### For Development Team:
```
1. Clone from GitHub
2. Run: dotnet run
3. Access: http://localhost:5000
```

---

## 🔧 Creating Desktop Shortcut (for Users)

### Method 1: Automatic (Installer)
- Run installer
- Desktop shortcut created automatically

### Method 2: Manual
```
1. Navigate to publish folder
2. Right-click: ContactManagementAPI.exe
3. Send to → Desktop (create shortcut)
4. Right-click shortcut → Rename
5. Change name to "Contact Management System"
6. Right-click → Properties → Advanced
7. Check "Run as administrator" (optional)
8. Click OK
```

---

## 🌐 Browser Access

### First Time:
```
Open browser and go to: http://localhost:5000
```

### Create Bookmark (for users):
```
1. Go to http://localhost:5000
2. Press Ctrl+D (or Cmd+D on Mac)
3. Save bookmark
4. Next time, click bookmark
```

---

## 📊 Directory Structure After Publishing

```
publish/
├── ContactManagementAPI.exe          (Main application)
├── Run.bat                           (Easy launcher)
├── README.txt                        (User instructions)
├── appsettings.json                  (Configuration)
├── appsettings.Release.json          (Release settings)
├── wwwroot/                          (Static files)
│   ├── css/
│   ├── js/
│   ├── uploads/
│   │   ├── photos/                   (Contact photos)
│   │   └── documents/                (Contact documents)
│   └── favicon.ico
├── Views/                            (Razor templates)
├── bin/                              (Dependencies)
└── [other .NET runtime files]
```

---

## ⚙️ Configuration for Users

### Port Change (if 5000 is in use):
Edit: `appsettings.json`
```json
"Urls": "http://localhost:5000"  // Change 5000 to 5001
```

### Database Location:
Default: `ContactManagementSystem.db` (in app directory)
Users can change in `appsettings.json`

### Connection String (SQL Server):
```json
"DefaultConnection": "Server=YOUR_SERVER;Database=ContactManagement;Trusted_Connection=true;"
```

---

## 🚨 Troubleshooting for Users

### Issue: "Port 5000 is in use"
```
Solution: Change port in appsettings.json
From: http://localhost:5000
To: http://localhost:5001
```

### Issue: "Cannot connect to application"
```
Solutions:
1. Check Windows Firewall → Allow port 5000
2. Restart application
3. Check if antivirus is blocking
4. Run as Administrator
```

### Issue: "Database not found"
```
Solution:
1. Delete existing database file
2. Restart application
3. Database will be recreated automatically
```

### Issue: "Permission denied when saving files"
```
Solution:
1. Run application as Administrator
2. Ensure folder has write permissions
3. Move to Documents or Program Files
```

---

## 📱 Accessing from Other Computers

### Same Network:
```
Replace localhost with computer IP:
http://192.168.1.100:5000
```

### Get Your Computer IP:
```powershell
ipconfig
```
Look for "IPv4 Address" (e.g., 192.168.x.x)

### Enable Remote Access:
Edit: `appsettings.json`
```json
"Urls": "http://*:5000"  // Listen on all interfaces
```

---

## 🔐 Security Notes

1. **Use HTTPS in Production**
   - Requires SSL certificate
   - Configure in appsettings.json

2. **Database Security**
   - Use strong credentials
   - Don't share connection strings
   - Regular backups

3. **File Upload Limits**
   - Max photo: 5 MB
   - Max document: 10 MB
   - Configure in Program.cs

---

## 📦 Version Updates

### Distribute Update:
```
1. Publish new version
2. Create new installer (increment version)
3. Users uninstall old version
4. Users install new version
```

### Keep User Data:
- Photo files in wwwroot/uploads/ are preserved
- Database file preserved during uninstall
- Users can backup before updating

---

## 🎓 Best Practices

### For Users:
✅ Create desktop shortcut  
✅ Bookmark in browser  
✅ Keep application window open  
✅ Regular database backups  
✅ Close cleanly (Ctrl+C, not force kill)  

### For Administrators:
✅ Test installer on clean machine  
✅ Document any configuration changes  
✅ Keep backup of database  
✅ Monitor logs for errors  
✅ Plan for updates  

### For Developers:
✅ Always test published version  
✅ Version control deployment scripts  
✅ Document breaking changes  
✅ Test on target Windows version  
✅ Maintain release notes  

---

## 📞 Support

**Issue Found?**
- GitHub Issues: https://github.com/abraham9486937737/Contact-Management-System/issues
- Include: Windows version, error message, steps to reproduce

**Want to Contribute?**
- Fork repository
- Create feature branch
- Submit pull request

---

## Quick Reference Commands

```powershell
# Development (run with auto-reload)
cd ContactManagementAPI
dotnet run

# Production (publish as self-contained)
dotnet publish -c Release -r win-x64 --self-contained

# Production (framework-dependent, smaller)
dotnet publish -c Release -r win-x64 --no-self-contained

# Run using PowerShell script
.\Publish-Application.ps1 -PublishType self-contained

# Create installer with Inno Setup
# 1. Open ContactManagementSystem-Setup.iss in Inno Setup
# 2. Build → Compile
```

---

## Summary

| Method | Size | Setup | Distribution | Best For |
|--------|------|-------|--------------|----------|
| **Batch File** | 150MB | None | Direct folder | Small teams |
| **Self-Contained EXE** | 150MB | Extract zip | Zip file | Easy distribution |
| **Framework-Dependent** | 15MB | Install .NET | Network | Small size |
| **Installer** | 80MB | Auto | Single exe | Professional delivery |

**Recommended:** Use **Self-Contained EXE with Installer** for professional deployment! 🚀

---

*For more details, see DEPLOYMENT_GUIDE.md*
