# Quick Start Script for Mobile Access
# Contact Management System

# Stop any running instance
Write-Host "Stopping existing application..." -ForegroundColor Yellow
Stop-Process -Name ContactManagementAPI -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2

# Get computer's IP address
Write-Host "Detecting network configuration..." -ForegroundColor Cyan
$ip = (Get-NetIPAddress -AddressFamily IPv4 | 
       Where-Object {$_.IPAddress -like '192.168.*' -or $_.IPAddress -like '10.*'} | 
       Select-Object -First 1).IPAddress

if (-not $ip) {
    Write-Host "❌ Could not detect IP address!" -ForegroundColor Red
    Write-Host "Make sure you are connected to WiFi/Network" -ForegroundColor Yellow
    exit
}

# Check/Create firewall rule
Write-Host "Checking firewall configuration..." -ForegroundColor Cyan
$firewallRule = Get-NetFirewallRule -DisplayName "Contact Management System" -ErrorAction SilentlyContinue
if (-not $firewallRule) {
    try {
        New-NetFirewallRule -DisplayName "Contact Management System" `
                           -Direction Inbound `
                           -LocalPort 5000 `
                           -Protocol TCP `
                           -Action Allow | Out-Null
        Write-Host "✅ Firewall rule created successfully" -ForegroundColor Green
    } catch {
        Write-Host "⚠️  Could not create firewall rule. You may need to run as Administrator." -ForegroundColor Yellow
    }
} else {
    Write-Host "✅ Firewall rule already exists" -ForegroundColor Green
}

# Display access information
Clear-Host
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "    CONTACT MANAGEMENT SYSTEM - MOBILE ACCESS" -ForegroundColor White
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""
Write-Host "Starting application..." -ForegroundColor Green
Write-Host ""
Write-Host "ACCESS FROM MOBILE DEVICE:" -ForegroundColor Yellow
Write-Host "   - Connect your mobile to the SAME WiFi network" -ForegroundColor White
Write-Host "   - Open mobile browser (Chrome/Safari)" -ForegroundColor White
Write-Host "   - Type this URL:" -ForegroundColor White
Write-Host ""
Write-Host "      http://$ip:5000" -ForegroundColor Green -BackgroundColor Black
Write-Host ""
Write-Host "ACCESS FROM THIS COMPUTER:" -ForegroundColor Yellow
Write-Host "      http://localhost:5000" -ForegroundColor Green -BackgroundColor Black
Write-Host ""
Write-Host "LOGIN CREDENTIALS:" -ForegroundColor Yellow
Write-Host "   Admin:  admin@example.com / Admin@123" -ForegroundColor White
Write-Host "   User:   user@example.com / User@123" -ForegroundColor White
Write-Host ""
Write-Host "NEW FEATURES:" -ForegroundColor Magenta
Write-Host "   - Grid view with all contact details" -ForegroundColor White
Write-Host "   - Select All checkbox in table header" -ForegroundColor White
Write-Host "   - Bulk delete multiple contacts" -ForegroundColor White
Write-Host "   - Touch-optimized for mobile devices" -ForegroundColor White
Write-Host ""
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "Press Ctrl+C to stop the application" -ForegroundColor Gray
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""

# Start the application
cd E:\Contact_Management_System\Published
.\ContactManagementAPI.exe
