@echo off
title Contact Management System
echo.
echo ========================================
echo  Contact Management System
echo ========================================
echo.
echo Starting application...
echo URL: http://localhost:5000
echo Press Ctrl+C to stop
echo.
cd /d "%~dp0"
start "" "http://localhost:5000"
ContactManagementAPI.exe
pause
