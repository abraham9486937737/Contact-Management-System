# Setup Guide

## One-Click Install (Recommended)

If you are on Windows and want the fastest setup:

1. Open the project root folder.
2. Double-click `Install_And_Run.bat`
3. Wait for installation to complete.
4. The app starts automatically at http://localhost:5000

What this installer does automatically:
- Uses the pre-published app package only if it is up-to-date; otherwise builds from current source
- Sets SQL Server instance to `.\SQLEXPRESS` by default
- Installs to `%LOCALAPPDATA%\ContactManagementSystem`
- Creates desktop shortcut: **Contact Management System**
- Creates launcher: `%LOCALAPPDATA%\ContactManagementSystem\Run-ContactManagementSystem.bat`

If your SQL Server instance is not `.\SQLEXPRESS`, run this in PowerShell from project root:

```powershell
.\Setup-Local-Install.ps1 -SqlServerInstance ".\YOUR_INSTANCE"
```

To always force a fresh build from source:

```powershell
.\Setup-Local-Install.ps1 -SourceMode Build
```

## Quick Start

This guide will help you get the Contact Management System up and running on your Windows machine.

## Step 1: Prerequisites

Make sure you have the following installed:

1. **.NET 8.0 SDK or later**
   - Download from: https://dotnet.microsoft.com/download
   - Verify installation: Open Command Prompt and run `dotnet --version`

2. **MS SQL Server (SQL Express recommended)**
   - Download SQL Server Express: https://www.microsoft.com/sql-server/sql-server-downloads
   - Download SQL Server Management Studio (SSMS): https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms

3. **Git** (Optional but recommended)
   - Download from: https://git-scm.com/download/win

4. **Visual Studio 2022** (Optional but recommended)
   - Download from: https://visualstudio.microsoft.com/downloads/
   - Or use VS Code: https://code.visualstudio.com/

## Step 2: Database Setup

### Option A: Using SQL Server Management Studio (SSMS)

1. Open SQL Server Management Studio
2. Connect to your SQL Server instance (usually `.\SQLEXPRESS`)
3. Right-click on "Databases" → "New Database"
4. Name it `ContactManagementDB`
5. Click "OK" to create

The database tables will be created automatically when you run the application for the first time.

### Option B: Using Command Line

If you prefer command line, you can let Entity Framework create the database automatically (see Step 4).

## Step 3: Update Connection String

1. Navigate to: `ContactManagementAPI\appsettings.json`
2. Locate the `ConnectionStrings` section
3. Update `DefaultConnection` with your SQL Server instance name:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\SQLEXPRESS;Database=ContactManagementDB;Trusted_Connection=true;TrustServerCertificate=true;Encrypt=false;"
}
```

**Common Server Names:**
- `.` or `localhost` - Local default instance
- `.\\SQLEXPRESS` - SQL Server Express (default)
- `.\\MSSQLSERVER` - Full SQL Server
- `COMPUTER_NAME\\SQLEXPRESS` - Named instance on another computer

## Step 4: Apply Database Migrations

1. Open Command Prompt or PowerShell
2. Navigate to the project directory:
   ```bash
   cd "e:\Contact_Management_System\ContactManagementAPI"
   ```

3. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

4. Apply Entity Framework migrations:
   ```bash
   dotnet ef database update
   ```

   This will create all necessary tables and seed initial data (contact groups).

## Step 5: Build the Application

```bash
dotnet build
```

## Step 6: Run the Application

```bash
dotnet run
```

The application will start and display:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[15]
      Application started. Press Ctrl+C to exit.
```

## Step 7: Access the Application

Open your web browser and navigate to:
- **HTTPS**: https://localhost:5001
- **HTTP**: http://localhost:5000

You should see the Contact Management System home page.

## Troubleshooting

### Database Connection Errors

**Error**: "Cannot connect to server.\SQLEXPRESS"

**Solution**:
1. Verify SQL Server is running
2. Check your SQL Server instance name:
   - Open SQL Server Configuration Manager
   - Check the instance name under "SQL Server Services"
3. Update the connection string accordingly

**Error**: "Login failed for user 'COMPUTER\username'"

**Solution**:
1. Ensure "Trusted_Connection=true" is set in the connection string
2. If using SQL Server authentication, use:
   ```
   Server=.\SQLEXPRESS;Database=ContactManagementDB;User Id=sa;Password=YourPassword;TrustServerCertificate=true;Encrypt=false;
   ```

### Entity Framework Migration Errors

**Error**: "The Entity Framework tools version does not match..."

**Solution**:
```bash
dotnet tool update --global dotnet-ef
```

### Port Already in Use

**Error**: "Unable to bind to http://0.0.0.0:5000 on the IPv4 loopback interface"

**Solution 1**: Use different ports
```bash
dotnet run --urls "http://localhost:5002;https://localhost:5003"
```

**Solution 2**: Kill the process using the port
```bash
netstat -ano | findstr :5000
taskkill /PID <PID> /F
```

### File Upload Issues

**Error**: "Access to the path 'wwwroot\uploads' is denied"

**Solution**:
1. Right-click `wwwroot` folder → Properties
2. Go to "Security" tab
3. Click "Edit" and allow "Full Control" for your user
4. Click "Apply" and "OK"

## Visual Studio Setup (Optional)

1. Open Visual Studio 2022
2. Click "Open a project or solution"
3. Navigate to and select `ContactManagementAPI.csproj`
4. Wait for NuGet packages to restore
5. In Package Manager Console (Tools → NuGet Package Manager → Package Manager Console):
   ```
   Update-Database
   ```
6. Press F5 or click "Start" to run

## VS Code Setup (Optional)

1. Open VS Code
2. Install extensions:
   - C# Dev Kit
   - SQL Server (mssql)
3. Open the `ContactManagementSystem` folder
4. Open integrated terminal (Ctrl+`)
5. Run commands as shown in Step 4-6 above

## Testing the Installation

### 1. Create a Contact
- Click "Add New Contact"
- Fill in the form fields
- Click "Create Contact"

### 2. Upload Photos
- Go to contact details
- Click "Photo Gallery"
- Drag-and-drop or select photos
- Verify photos appear in the gallery

### 3. Upload Documents
- Click "Manage Documents"
- Select a document type
- Upload a test document
- Verify document appears in the list

### 4. Search Contacts
- Enter a search term in the search box
- Verify results are filtered correctly

## Production Deployment

For deploying to production, refer to the [DEPLOYMENT.md](DEPLOYMENT.md) guide.

## Next Steps

1. Customize contact groups
2. Modify styling in `wwwroot\css\style.css`
3. Add authentication (see Security section in README.md)
4. Set up automated backups
5. Configure SMTP for email notifications (future feature)

## Support

If you encounter any issues:
1. Check the Troubleshooting section above
2. Review application logs in the console
3. Check SQL Server logs in SQL Server Management Studio
4. Open an issue in the GitHub repository

---

**Need Help?** Refer to the main [README.md](../README.md) for more information.
