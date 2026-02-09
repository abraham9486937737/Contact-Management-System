@echo off
REM ========================================
REM Contact Management System - Quick Start
REM ========================================

echo.
echo ╔════════════════════════════════════════════════════════════╗
echo ║     Contact Management System - Starting Application       ║
echo ╚════════════════════════════════════════════════════════════╝
echo.

cd /d "%~dp0ContactManagementAPI"

echo Starting application...
echo.
echo Application will be available at: http://localhost:5000
echo Press Ctrl+C to stop the application
echo.
echo ────────────────────────────────────────────────────────────
echo.

dotnet run --configuration Release

echo.
echo ────────────────────────────────────────────────────────────
echo Application has stopped. Closing...
echo.
pause
