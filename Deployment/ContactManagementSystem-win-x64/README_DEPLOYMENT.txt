CONTACT MANAGEMENT SYSTEM - SHAREABLE DEPLOYMENT
===============================================

What is included
----------------
- ContactManagementAPI.exe (self-contained application)
- Run.bat (start on localhost)
- Run-Mobile-Access.bat (start for access from other devices in same Wi-Fi/LAN)

System requirements (receiver)
------------------------------
- Windows 10/11 (64-bit)
- SQL Server instance (SQLEXPRESS recommended)
- Any browser: Chrome, Edge, Firefox, Safari

How to run (same PC)
--------------------
1) Extract this full folder (do NOT run from zip).
2) Double-click Run.bat
3) Browser opens at: http://localhost:5000

How to run (other devices)
--------------------------
1) On host PC, double-click Run-Mobile-Access.bat
2) Note the host PC IPv4 shown in command window
3) On phone/laptop in same network, open: http://HOST_IP:5000
4) Allow firewall prompt if shown

Default SQL Server connection
-----------------------------
The app expects: .\SQLEXPRESS

If receiver has different SQL instance:
- Open appsettings.json
- Update ConnectionStrings > DefaultConnection
- Example:
  Server=.\MSSQLSERVER;Database=ContactManagementDB;Trusted_Connection=true;TrustServerCertificate=true;Encrypt=false;

Important
---------
- This package is Windows-only (win-x64).
- App works in any browser; for "any device" the host PC must stay running.
