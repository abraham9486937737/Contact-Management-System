@echo off
echo ================================================
echo   Contact Management System - Mobile Access
echo ================================================
echo.
echo Starting application with network access...
echo This allows you to access from your mobile device!
echo.
echo Default URL: http://localhost:5000
echo Network URL: http://YOUR_IP:5000
echo.
echo To find your IP address, look for "IPv4 Address" below:
echo.
ipconfig | findstr /i "IPv4"
echo.
echo ================================================
echo.
echo Starting application...
echo Press Ctrl+C to stop the application
echo.

cd /d "%~dp0Published"
set ASPNETCORE_URLS=http://0.0.0.0:5000
ContactManagementAPI.exe

pause
