# Generate Simple Icon Placeholders for PWA
# This creates basic colored squares with text as placeholders
# You can replace these with actual designed icons later

$iconSizes = @(72, 96, 128, 144, 152, 192, 384, 512)
$outputPath = "ContactManagementAPI\wwwroot"

Write-Host "Generating PWA icon placeholders..." -ForegroundColor Green

# Check if System.Drawing is available
Add-Type -AssemblyName System.Drawing

foreach ($size in $iconSizes) {
    $bitmap = New-Object System.Drawing.Bitmap($size, $size)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    
    # Fill with gradient-like green background
    $brush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(76, 175, 80))
    $graphics.FillRectangle($brush, 0, 0, $size, $size)
    
    # Add white text
    $text = "CMS"
    $font = New-Object System.Drawing.Font("Arial", [int]($size * 0.25), [System.Drawing.FontStyle]::Bold)
    $textBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::White)
    
    # Center the text
    $textSize = $graphics.MeasureString($text, $font)
    $x = ($size - $textSize.Width) / 2
    $y = ($size - $textSize.Height) / 2
    
    $graphics.DrawString($text, $font, $textBrush, $x, $y)
    
    # Save the image
    $filename = "$outputPath\icon-$size.png"
    $bitmap.Save($filename, [System.Drawing.Imaging.ImageFormat]::Png)
    
    Write-Host "  Created: icon-$size.png" -ForegroundColor Cyan
    
    # Cleanup
    $graphics.Dispose()
    $bitmap.Dispose()
    $brush.Dispose()
    $textBrush.Dispose()
    $font.Dispose()
}

Write-Host "`nAll icon placeholders generated successfully!" -ForegroundColor Green
Write-Host "Note: These are placeholder icons. Replace them with professionally designed icons for production." -ForegroundColor Yellow
