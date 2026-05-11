# Contact Management System - Tools and Technologies Used (Beginner Guide)

This document is written for beginners, including students who are new to software applications.

Goal of this guide:
- Explain what tools and technologies are used in this project.
- Explain why each one is used.
- Show where each one appears in this project.
- Help a new person make safe changes without breaking the app.

## 1) Big Picture: How This App Works

This is a web application.

Flow:
1. User opens a URL in browser.
2. Browser sends request to the server app.
3. Server app reads or writes data from database.
4. Server app returns a page (HTML/CSS/JS).
5. User sees and interacts with contacts.

Main layers in this project:
- Frontend: what user sees (pages, styles, buttons, tables).
- Backend: business logic and request handling.
- Database: stores contacts, users, groups, photos, documents metadata.
- Hosting: IIS runs the app on Windows.

## 2) Core Technologies

### C# and .NET 8

What it is:
- C# is the programming language.
- .NET 8 is the runtime platform that runs the app.

Why used here:
- Strong, modern language with good tooling.
- Works well for enterprise-style web apps.

Where to see it:
- ContactManagementAPI/ContactManagementAPI.csproj

### ASP.NET Core MVC

What it is:
- A web framework using Model, View, Controller pattern.

Why used here:
- Keeps code organized.
- Easy to maintain pages, logic, and data flow separately.

Where to see it:
- Controllers in ContactManagementAPI/Controllers
- Views in ContactManagementAPI/Views
- Startup configuration in ContactManagementAPI/Program.cs

### Razor Views

What it is:
- Server-rendered page templates with C# + HTML.

Why used here:
- Fast to build data-driven UI pages.
- Good for CRUD-style applications.

Where to see it:
- ContactManagementAPI/Views/**/*.cshtml

## 3) Database and Data Access

### Entity Framework Core (EF Core)

What it is:
- ORM (Object-Relational Mapper): write C# objects instead of raw SQL most of the time.

Why used here:
- Speeds up development.
- Makes database code cleaner.

Where to see it:
- ContactManagementAPI/Data/ApplicationDbContext.cs
- ContactManagementAPI/Models
- ContactManagementAPI/Controllers

### SQLite and SQL Server support

What it is:
- SQLite: file-based database (good for easy local deployment).
- SQL Server: full database server (good for larger or managed environments).

Why used here:
- Flexibility: same app can run in simple local mode or server mode.

Where to see selection logic:
- ContactManagementAPI/Program.cs

### EF Tools package

What it is:
- Tools for migrations and schema updates.

Where listed:
- ContactManagementAPI/ContactManagementAPI.csproj

## 4) Authentication and Authorization

### Session authentication

What it is:
- After login, user session is stored on server side.

Why used here:
- Simple and effective for intranet/local use.

Where to see it:
- ContactManagementAPI/Controllers/AccountController.cs
- ContactManagementAPI/Program.cs

### Password hashing

What it is:
- Password is stored as hash, not plain text.

Why used here:
- Security best practice.

Where to see it:
- ContactManagementAPI/Controllers/AccountController.cs
- ContactManagementAPI/Services/SeedData.cs

### Role/rights control

What it is:
- Users have rights; controllers/actions require rights.

Why used here:
- Restricts sensitive actions like user/group management and delete operations.

Where to see it:
- ContactManagementAPI/Security/RequireRightAttribute.cs
- ContactManagementAPI/Services/AuthorizationService.cs
- ContactManagementAPI/Controllers/AdminController.cs

## 5) UI and Frontend Tools

### HTML, CSS, JavaScript

What they do:
- HTML: page structure.
- CSS: styling.
- JavaScript: browser interactions.

Where to see it:
- ContactManagementAPI/wwwroot/css
- ContactManagementAPI/wwwroot/js
- ContactManagementAPI/Views

### Bootstrap

What it is:
- UI framework for layout, responsive design, and components.

Where to see it:
- ContactManagementAPI/wwwroot/lib/bootstrap

### Font Awesome

What it is:
- Icon library used for buttons and visual hints.

Where to see it:
- Referenced in layout/view files.

## 6) Import/Export and File Features

### CsvHelper

Used for:
- Reading and writing CSV contacts.

### EPPlus

Used for:
- Reading and writing Excel files.

### QuestPDF

Used for:
- Exporting contacts to PDF.

Where to see these in project:
- ContactManagementAPI/Services/ImportExportService.cs
- Package references in ContactManagementAPI/ContactManagementAPI.csproj

### File upload system

Used for:
- Storing contact photos and documents.

Where to see it:
- ContactManagementAPI/Services/FileUploadService.cs
- ContactManagementAPI/wwwroot/uploads

## 7) Hosting and Deployment

### IIS (Windows hosting)

What it is:
- Web server on Windows.

Why used here:
- Run app with a permanent URL on local machine without running dotnet run each time.

Where to see setup automation:
- Setup-IIS-Hosting.ps1
- DAY_TO_DAY_USE_GUIDE.md

### ASP.NET Core Hosting Bundle

What it is:
- Required IIS module to host ASP.NET Core apps.

Why important:
- Without it, IIS returns errors like HTTP 500.19.

### Docker and Render

What they are:
- Docker: containerizes app.
- Render: cloud platform using Docker deploy config.

Where to see it:
- Dockerfile
- render.yaml

### Inno Setup

What it is:
- Windows installer creator.

Where to see it:
- ContactManagementSystem-Setup.iss

## 8) Daily Developer/Operator Tools

### PowerShell

Used for:
- Setup, deployment, IIS control, troubleshooting.

### .NET CLI

Common commands:
- dotnet restore
- dotnet build
- dotnet publish
- dotnet run

### IIS commands

Common commands:
- iisreset
- Start-Website -Name "ContactManagementSystem"
- Stop-Website -Name "ContactManagementSystem"

## 9) Beginner-Friendly Change Guide

If you are new and want to modify the project safely, follow this order.

1. Understand where to edit.
- UI text/layout: edit files in ContactManagementAPI/Views and ContactManagementAPI/wwwroot/css.
- Business logic: edit files in ContactManagementAPI/Controllers and ContactManagementAPI/Services.
- Database model: edit files in ContactManagementAPI/Models and ApplicationDbContext.

2. Make one small change at a time.
- Change one feature.
- Run and test.
- Then do next change.

3. Test after each change.
- Open http://localhost:8080
- Verify page loads.
- Verify create/edit/import/export flows still work.

4. Redeploy safely after code change.
- Use Setup-IIS-Hosting.ps1 with ForceRecreateSite.

5. Keep backups with Git.
- Commit before major edits.
- Write clear commit messages.

## 10) File Map for Learners

Start learning from these files in order:
1. ContactManagementAPI/Program.cs
2. ContactManagementAPI/Controllers/HomeController.cs
3. ContactManagementAPI/Views/Home/Index.cshtml
4. ContactManagementAPI/Data/ApplicationDbContext.cs
5. ContactManagementAPI/Services/ImportExportService.cs
6. Setup-IIS-Hosting.ps1
7. DAY_TO_DAY_USE_GUIDE.md
8. USER_GUIDE.md

## 11) Quick Glossary

- Framework: a ready-made structure to build apps faster.
- Runtime: software that executes your code.
- ORM: tool that maps database tables to code classes.
- Migration: database schema change history.
- Deployment: moving app to runnable environment.
- Hosting: running app continuously and exposing URL.
- Session: temporary user login state.

## 12) Summary

This project uses a practical and teachable stack:
- .NET 8 + ASP.NET Core MVC for web app logic
- EF Core + SQLite/SQL Server for data
- CsvHelper/EPPlus/QuestPDF for import/export
- IIS for always-on local hosting
- PowerShell scripts for repeatable setup and redeploy

A beginner can learn this project by following the file map and making small tested changes.
