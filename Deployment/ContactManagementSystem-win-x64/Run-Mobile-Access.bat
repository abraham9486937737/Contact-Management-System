@echo off
title Contact Management System - Mobile Access
echo.
echo ========================================
echo  Contact Management System - Mobile Access
echo ========================================
echo.
echo This mode allows access from other devices on same network.
echo.
echo Your IPv4 addresses:
ipconfig | findstr /i "IPv4"
echo.
echo Open from another device: http://YOUR_PC_IP:5000
echo Press Ctrl+C to stop
echo.
cd /d "%~dp0"
set ASPNETCORE_URLS=http://0.0.0.0:5000
ContactManagementAPI.exe
pause
