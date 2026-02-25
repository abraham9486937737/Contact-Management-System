@echo off
title Contact Management System Installer

echo.
echo ========================================
echo  Contact Management System Installer
echo ========================================
echo.

powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0Setup-Local-Install.ps1"

if errorlevel 1 (
  echo.
  echo Installation failed. Please review the error above.
  pause
  exit /b 1
)

echo.
echo Installation completed successfully.
pause