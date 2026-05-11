# Contact Management System - User Guide

This guide is for end users who use the application day to day.

## 1. Open the Application

- Local URL: http://localhost:8080
- If your system uses a different port, use that port instead.

## 2. Login

Use one of the configured accounts:

- Username: admin
- Password: Admin@123

Super admin account:

- Username: abrahamcbe@gmail.com
- Password: M@ld1ves

## 3. Home Screen

After login, open All Contacts to view contact records.

Main actions available from top and page buttons:

- Add New Contact
- Dashboard
- Find Duplicates
- Import
- Export
- Users, Groups, History (admin features)

## 4. Add a New Contact

1. Click Add New Contact.
2. Fill basic details: name, phone, email, city, group.
3. Fill optional details: WhatsApp, address, IDs, bank details.
4. Upload photo/documents if needed.
5. Click Save.

## 5. Edit or View Contact

1. In All Contacts, locate the contact row.
2. Click View to open full details.
3. Click Edit to update fields.
4. Click Save.

## 6. Search, Filter, Sort, Pagination

In All Contacts:

- Use search/filter boxes under table headers.
- Click column headers to sort.
- Use page controls to move between pages.

## 7. Import Contacts

1. Click Import.
2. Choose file type (CSV or Excel).
3. Select file and upload.
4. Review import result (inserted or skipped duplicates).

## 8. Export Contacts

1. Click Export.
2. Choose Excel, CSV, or PDF.
3. File downloads to your browser download folder.

## 9. Dashboard and Duplicate Detection

- Dashboard shows key statistics.
- Find Duplicates helps detect similar or repeated contacts.

## 10. Logout

Click Logout at the top-right when finished.

## 11. Common Issues

### Cannot open URL

- Confirm IIS site is running.
- Open http://localhost:8080 again.

### Login failed

- Check username and password spelling.
- Ensure CAPS LOCK is off.

### Image not showing (404)

- Re-upload image from Edit Contact.

### Slow or failed page load

- Refresh browser once.
- Retry after a few seconds.

## 12. Quick Admin Commands

Run in Administrator PowerShell when needed:

```powershell
Start-Website -Name "ContactManagementSystem"
Stop-Website -Name "ContactManagementSystem"
Set-Location E:\Contact_Management_System
.\Setup-IIS-Hosting.ps1 -SiteName "ContactManagementSystem" -AppPoolName "ContactManagementSystemPool" -Port 8080 -ForceRecreateSite
```
