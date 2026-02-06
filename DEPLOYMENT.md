# Deployment Guide

## Overview

This guide covers deploying the Contact Management System to different environments.

## Local Development Deployment

See [SETUP.md](SETUP.md) for local development setup.

## IIS Deployment (Windows Server)

### Prerequisites

- Windows Server 2016 or later
- IIS 10.0 or later
- .NET Hosting Bundle
- MS SQL Server

### Step 1: Install .NET Hosting Bundle

1. Download .NET 8.0 Hosting Bundle from: https://dotnet.microsoft.com/download/dotnet/8.0
2. Run the installer
3. Restart IIS: 
   ```bash
   iisreset
   ```

### Step 2: Publish Application

In the application directory:
```bash
dotnet publish -c Release -o "C:\inetpub\wwwroot\ContactManagement"
```

### Step 3: Create IIS Application

1. Open IIS Manager
2. Right-click "Sites" → "Add Website"
3. Configure:
   - Site name: Contact Management
   - Physical path: `C:\inetpub\wwwroot\ContactManagement`
   - Binding: http, Port 80 (or 443 for HTTPS)
4. Click "OK"

### Step 4: Configure Application Pool

1. Right-click Application Pool → "Set Application Pool Defaults"
2. Set:
   - .NET CLR version: No Managed Code
   - Managed pipeline mode: Integrated
3. Click "OK"

### Step 5: Update Connection String

1. Edit `appsettings.json` in the published directory
2. Update the SQL Server connection string for your server
3. Set appropriate file permissions

### Step 6: Configure Folder Permissions

1. Right-click the website folder → Properties
2. Security tab → Edit
3. Give "IIS_IUSRS" and "NETWORK SERVICE" Full Control

### Step 7: Verify Installation

Navigate to your website URL in a browser.

## Azure App Service Deployment

### Prerequisites

- Azure account
- Azure CLI (optional)

### Step 1: Create App Service

1. Go to Azure Portal
2. Create new "App Service"
3. Configure:
   - Name: contact-management-system
   - Runtime: ASP.NET Core 8.0
   - Region: Your region

### Step 2: Create Azure SQL Database

1. Create "SQL Database"
2. Configure connection string
3. Note the connection string

### Step 3: Publish from Visual Studio

1. Right-click project → "Publish"
2. Select "Azure" → "Create new"
3. Follow wizard:
   - App Service Plan
   - Select created App Service
   - Configure database

### Step 4: Configure Application Settings

1. In Azure Portal → App Service → Configuration
2. Add Connection String:
   ```
   Name: ConnectionStrings:DefaultConnection
   Value: [Your SQL Database connection string]
   ```

### Step 5: Verify Deployment

Navigate to your App Service URL.

## Docker Deployment (Optional)

### Create Dockerfile

Create `Dockerfile` in project root:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ContactManagementAPI.csproj", ""]
RUN dotnet restore "ContactManagementAPI.csproj"
COPY . .
RUN dotnet build "ContactManagementAPI.csproj" -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/build .
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "ContactManagementAPI.dll"]
```

### Build and Run

```bash
docker build -t contact-management:latest .
docker run -p 80:80 contact-management:latest
```

## Database Backup Strategy

### Manual Backup (SQL Server Management Studio)

1. Right-click Database → Tasks → Back Up
2. Choose backup location
3. Click "OK"

### Automated Backup (SQL Server Agent)

1. Set up SQL Server Agent job
2. Schedule daily/weekly backups
3. Store in secure location

### Azure SQL Database Backup

- Automatic backups every 5-10 minutes
- Long-term retention (7-35 days)
- Geo-redundant storage available

## SSL/TLS Configuration

### Self-Signed Certificate (Development)

```bash
dotnet dev-certs https --trust
```

### Let's Encrypt (Production)

1. Use Certbot: https://certbot.eff.org/
2. Or configure in Azure/IIS with automatic renewal

## Environment-Specific Configuration

Create separate appsettings files:

- `appsettings.Development.json`
- `appsettings.Production.json`
- `appsettings.Staging.json`

Set environment variable:
```bash
ASPNETCORE_ENVIRONMENT=Production
```

## Monitoring & Logging

### Application Insights (Azure)

1. Add to `Program.cs`:
```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

2. Configure in appsettings:
```json
"ApplicationInsights": {
  "InstrumentationKey": "your-key"
}
```

### File Logging

1. Install package:
```bash
dotnet add package Serilog.Extensions.Logging.File
```

2. Configure logging in `Program.cs`

## Performance Optimization

### 1. Database Indexing

Ensure indexes on frequently queried columns:
- FirstName
- LastName
- Email
- Mobile numbers

### 2. Caching

Add distributed caching:
```csharp
builder.Services.AddStackExchangeRedisCache(options => 
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});
```

### 3. CDN Configuration

For static assets, configure CDN:
- Use Azure CDN
- Or CloudFlare

## Security Checklist

- [ ] Enable HTTPS only
- [ ] Configure CORS properly
- [ ] Implement authentication/authorization
- [ ] Use strong SQL Server passwords
- [ ] Configure firewall rules
- [ ] Enable SQL Server encryption
- [ ] Regular security updates
- [ ] Implement rate limiting
- [ ] Add Web Application Firewall (WAF)
- [ ] Regular security audits

## Troubleshooting

### Application Won't Start

1. Check event viewer for errors
2. Verify .NET Runtime is installed
3. Check connection string
4. Review application logs

### Database Connection Issues

1. Verify SQL Server is running
2. Check firewall rules (for remote servers)
3. Verify login credentials
4. Check network connectivity

### Performance Issues

1. Monitor CPU/Memory usage
2. Check SQL Server queries
3. Review application logs
4. Profile with VS Profiler

### File Upload Issues

1. Verify folder permissions
2. Check disk space
3. Review IIS upload limits
4. Check application logs

## Rollback Procedure

### If Deployment Fails

1. IIS: Restore previous version from backup
2. Azure: Use Deployment Center → Deployments → Redeploy
3. Docker: Use previous image version

## Support & Maintenance

- Regular backups (daily minimum)
- Monthly security updates
- Quarterly performance reviews
- Annual disaster recovery testing

---

For questions or issues, refer to the main [README.md](README.md) or [SETUP.md](SETUP.md).
