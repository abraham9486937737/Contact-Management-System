$ExcelPath = "e:\Contact_Management_System\Sample_Contacts.xlsx"

# Import the EPPlus assembly
Add-Type -Path "E:\Contact_Management_System\Published\EPPlus.dll"

# Create Excel package
$ExcelPackage = New-Object OfficeOpenXml.ExcelPackage

# Add worksheet
$worksheet = $ExcelPackage.Workbook.Worksheets.Add("Contacts")

# Add headers
$headers = @("FirstName", "LastName", "NickName", "Email", "Mobile1", "Mobile2", "Mobile3", "WhatsAppNumber", "Address", "City", "State", "PostalCode", "Country", "OtherDetails")
for ($i = 0; $i -lt $headers.Length; $i++) {
    $worksheet.Cells[1, ($i + 1)].Value = $headers[$i]
}

# Sample data
$sampleData = @(
    @("John", "Doe", "Johnny", "john.doe@example.com", "+1-555-0101", "+1-555-0102", "", "+1-555-0101", "123 Main St", "New York", "NY", "10001", "USA", "Senior Manager"),
    @("Jane", "Smith", "JS", "jane.smith@example.com", "+1-555-0201", "+1-555-0202", "", "+1-555-0201", "456 Oak Ave", "Los Angeles", "CA", "90001", "USA", "Marketing Head"),
    @("Michael", "Johnson", "Mike", "michael.j@example.com", "+1-555-0301", "+1-555-0302", "", "+1-555-0301", "789 Pine Rd", "Chicago", "IL", "60601", "USA", "Software Engineer"),
    @("Sarah", "Williams", "Sarah W", "sarah.williams@example.com", "+1-555-0401", "+1-555-0402", "", "+1-555-0401", "321 Elm St", "Houston", "TX", "77001", "USA", "Project Manager"),
    @("David", "Brown", "DB", "david.brown@example.com", "+1-555-0501", "+1-555-0502", "", "+1-555-0501", "654 Maple Dr", "Phoenix", "AZ", "85001", "USA", "Business Analyst"),
    @("Emily", "Davis", "Emily D", "emily.davis@example.com", "+1-555-0601", "+1-555-0602", "", "+1-555-0601", "987 Cedar Ln", "Philadelphia", "PA", "19101", "USA", "HR Specialist"),
    @("Robert", "Miller", "Bob", "robert.miller@example.com", "+1-555-0701", "+1-555-0702", "", "+1-555-0701", "135 Birch Ct", "San Antonio", "TX", "78201", "USA", "Financial Advisor"),
    @("Lisa", "Wilson", "LW", "lisa.wilson@example.com", "+1-555-0801", "+1-555-0802", "", "+1-555-0801", "246 Walnut St", "San Diego", "CA", "92101", "USA", "Sales Executive"),
    @("James", "Moore", "Jim", "james.moore@example.com", "+1-555-0901", "+1-555-0902", "", "+1-555-0901", "369 Oak Pl", "Dallas", "TX", "75201", "USA", "Operations Manager"),
    @("Patricia", "Taylor", "Pat", "patricia.taylor@example.com", "+1-555-1001", "+1-555-1002", "", "+1-555-1001", "482 Pine Ave", "San Jose", "CA", "95101", "USA", "Administrative Officer")
)

# Add data rows
for ($row = 0; $row -lt $sampleData.Length; $row++) {
    for ($col = 0; $col -lt $sampleData[$row].Length; $col++) {
        $worksheet.Cells[($row + 2), ($col + 1)].Value = $sampleData[$row][$col]
    }
}

# Auto-fit columns
$worksheet.Cells[1, 1, ($sampleData.Length + 1), $headers.Length].AutoFitColumns()

# Save the file
$ExcelPackage.SaveAs($ExcelPath)
$ExcelPackage.Dispose()

Write-Host "Sample Excel file created: $ExcelPath"
