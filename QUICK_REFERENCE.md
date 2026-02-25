# Quick Reference Guide

## Common Commands

### Initial Setup
```bash
# Navigate to project
cd "e:\Contact_Management_System\ContactManagementAPI"

# Restore NuGet packages
dotnet restore

# Create database
dotnet ef database update

# Build project
dotnet build
```

### Running the Application
```bash
# Run with development settings
dotnet run

# Run with specific URLs
dotnet run --urls "http://localhost:5000;https://localhost:5001"

# Run in watch mode (auto-reload on changes)
dotnet watch run
```

### Database Commands
```bash
# Create migration
dotnet ef migrations add [MigrationName]

# Update database with latest migration
dotnet ef database update

# Revert to previous migration
dotnet ef database update [PreviousMigrationName]

# Remove last migration
dotnet ef migrations remove

# View SQL from migrations
dotnet ef migrations script
```

### Publishing
```bash
# Publish for release
dotnet publish -c Release -o "./publish"

# Publish for specific platform
dotnet publish -c Release -r win-x64 -o "./publish"
```

### Cleaning
```bash
# Clean build artifacts
dotnet clean

# Remove node_modules if applicable
Remove-Item -Recurse -Force node_modules
```

---

## Project Locations

| Item | Path |
|------|------|
| **Main Project** | `ContactManagementAPI/` |
| **Controllers** | `ContactManagementAPI/Controllers/` |
| **Models** | `ContactManagementAPI/Models/` |
| **Views** | `ContactManagementAPI/Views/` |
| **CSS** | `ContactManagementAPI/wwwroot/css/` |
| **JavaScript** | `ContactManagementAPI/wwwroot/js/` |
| **Uploads** | `ContactManagementAPI/wwwroot/uploads/` |
| **Config** | `ContactManagementAPI/appsettings.json` |
| **Docs** | `README.md`, `SETUP.md`, `DEPLOYMENT.md` |

---

## Important Files

| File | Purpose |
|------|---------|
| `Program.cs` | Application entry point |
| `appsettings.json` | Configuration settings |
| `ContactManagementAPI.csproj` | Project dependencies |
| `ApplicationDbContext.cs` | Database context |
| `HomeController.cs` | Contact operations |
| `PhotoController.cs` | Photo management |
| `DocumentController.cs` | Document management |

---

## Browser Access

| URL | Purpose |
|-----|---------|
| `https://localhost:5001` | HTTPS (default) |
| `http://localhost:5000` | HTTP alternative |
| `https://localhost:44318` | IIS Express HTTPS |
| `http://localhost:7089` | IIS Express HTTP |

---

## Troubleshooting Commands

```bash
# Check .NET version
dotnet --version

# List installed runtimes
dotnet --list-runtimes

# List installed SDKs
dotnet --list-sdks

# Check if port is in use
netstat -ano | findstr :5000

# Kill process using port
taskkill /PID [PID] /F

# Clear NuGet cache
dotnet nuget locals all --clear

# Reset Entity Framework
dotnet ef database drop --force
dotnet ef database update
```

---

## Git Commands

```bash
# Initialize repository
git init

# Check status
git status

# Stage changes
git add .

# Commit changes
git commit -m "Your message here"

# View commit history
git log --oneline

# Add remote
git remote add origin [URL]

# Push to remote
git push -u origin main

# Fetch updates
git fetch

# Pull updates
git pull

# Create branch
git branch [branch-name]

# Switch branch
git checkout [branch-name]

# Delete branch
git branch -d [branch-name]
```

---

## SQL Server Management

```bash
# Connect to SSMS
# Server: .\SQLEXPRESS
# Authentication: Windows Authentication

# Common SQL Queries

-- View all contacts
SELECT * FROM Contacts;

-- View contacts with groups
SELECT c.*, cg.Name as GroupName
FROM Contacts c
LEFT JOIN ContactGroups cg ON c.GroupId = cg.Id;

-- Count contacts
SELECT COUNT(*) as TotalContacts FROM Contacts;

-- View groups
SELECT * FROM ContactGroups;

-- View photos
SELECT * FROM ContactPhotos;

-- View documents
SELECT * FROM ContactDocuments;

-- Delete all contacts (careful!)
DELETE FROM Contacts;

-- Get largest documents
SELECT TOP 10 * FROM ContactDocuments
ORDER BY FileSize DESC;
```

---

## File Upload Paths

```
wwwroot/uploads/
├── photos/
│   └── [ContactId]_[Timestamp].jpg
│   └── [ContactId]_[Timestamp].png
│   └── etc.
└── documents/
    └── [ContactId]_[Timestamp].pdf
    └── [ContactId]_[Timestamp].docx
    └── etc.
```

---

## Environment Variables

```bash
# Set environment
$env:ASPNETCORE_ENVIRONMENT="Development"

# Verify environment
$env:ASPNETCORE_ENVIRONMENT

# Set for session
[Environment]::SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production", "User")
```

---

## Configuration Settings

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=ContactManagementDB;..."
  },
  "FileUpload": {
    "MaxPhotoSize": 5242880,
    "MaxDocumentSize": 10485760
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

---

## Performance Monitoring

```bash
# Monitor CPU/Memory
Get-Process | Where-Object {$_.Name -like "*dotnet*"}

# Check active ports
netstat -ab | findstr "5000\|5001"

# Get process details
Get-Process -Name dotnet | Select-Object Id, ProcessName, WorkingSet, CPU
```

---

## Extensions to Install (VS Code)

- C# Dev Kit (ms-dotnettools.csharp)
- SQL Server (mssql)
- REST Client (humao.rest-client)
- Prettier - Code formatter (esbenp.prettier-vscode)
- Markdown Preview Enhanced (shd101wyy.markdown-preview-enhanced)

---

## Keyboard Shortcuts

### Visual Studio / VS Code
| Shortcut | Action |
|----------|--------|
| `Ctrl+K Ctrl+C` | Comment code |
| `Ctrl+K Ctrl+U` | Uncomment code |
| `Ctrl+Shift+B` | Build solution |
| `F5` | Start debugging |
| `Ctrl+.` | Quick actions |
| `Ctrl+F5` | Run without debugging |
| `Shift+Alt+D` | Show debug console |

### Browser DevTools
| Shortcut | Action |
|----------|--------|
| `F12` | Open DevTools |
| `Ctrl+Shift+I` | Inspect element |
| `Ctrl+Shift+Delete` | Clear cache |
| `Ctrl+Shift+M` | Toggle device mode |

---

## Documentation Map

```
Project Root/
├── README.md                 ← Start here for overview
├── SETUP.md                  ← Installation instructions
├── DEPLOYMENT.md             ← Production deployment
├── API_DOCUMENTATION.md      ← Technical details
├── PROJECT_SUMMARY.md        ← Complete summary
└── QUICK_REFERENCE.md        ← This file
```

---

## Support Resources

- **Official .NET Documentation**: https://docs.microsoft.com/dotnet/
- **Entity Framework Core**: https://docs.microsoft.com/en-us/ef/core/
- **ASP.NET Core**: https://docs.microsoft.com/en-us/aspnet/core/
- **SQL Server**: https://docs.microsoft.com/en-us/sql/
- **Git Documentation**: https://git-scm.com/doc

---

## Tips & Tricks

1. **Use Source Control**: Commit after each feature
2. **Test Locally**: Always test before pushing
3. **Keep Backups**: Regular database backups
4. **Monitor Performance**: Use browser DevTools
5. **Check Logs**: Application logs help troubleshoot
6. **Use Watch Mode**: `dotnet watch run` for development
7. **Format Code**: Keep consistent formatting
8. **Add Comments**: Document complex logic
9. **Update Regularly**: Keep packages current
10. **Use Environment Configs**: Different settings per environment

---

## Common Issues & Solutions

| Issue | Solution |
|-------|----------|
| Port already in use | Kill process: `taskkill /PID [PID] /F` |
| Database not found | Check connection string in appsettings.json |
| CSS not loading | Clear cache: `Ctrl+Shift+Delete` |
| Build fails | Run: `dotnet clean` then `dotnet build` |
| Migration issues | Run: `dotnet ef migrations remove` then `update` |
| File upload fails | Check folder permissions on wwwroot |
| HTTPS error | Run: `dotnet dev-certs https --trust` |

---

## Useful PowerShell Functions

```powershell
# Function to restart IIS
function Restart-IIS {
    iisreset
    Write-Host "IIS restarted"
}

# Function to open project folder
function Open-Project {
    explorer "e:\Contact_Management_System"
}

# Function to build and run
function Build-Run {
    dotnet clean
    dotnet build
    dotnet run
}
```

---

**Last Updated**: February 6, 2026

For more detailed information, refer to the main documentation files.
