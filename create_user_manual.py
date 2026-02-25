#!/usr/bin/env python3
"""
Create a comprehensive User Manual PDF for Contact Management System
"""

from reportlab.lib.pagesizes import letter, A4
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib.units import inch
from reportlab.lib.enums import TA_CENTER, TA_LEFT, TA_JUSTIFY
from reportlab.platypus import SimpleDocTemplate, Paragraph, Spacer, PageBreak, Table, TableStyle, Image
from reportlab.lib import colors
from datetime import datetime

# Create PDF
pdf_file = "Contact_Management_System_User_Manual.pdf"
doc = SimpleDocTemplate(pdf_file, pagesize=letter, topMargin=0.5*inch, bottomMargin=0.5*inch)

# Container for PDF elements
elements = []

# Get styles
styles = getSampleStyleSheet()

# Create custom styles
title_style = ParagraphStyle(
    'CustomTitle',
    parent=styles['Heading1'],
    fontSize=28,
    textColor=colors.HexColor('#667eea'),
    spaceAfter=30,
    alignment=TA_CENTER,
    fontName='Helvetica-Bold'
)

heading_style = ParagraphStyle(
    'CustomHeading',
    parent=styles['Heading2'],
    fontSize=14,
    textColor=colors.HexColor('#764ba2'),
    spaceAfter=12,
    spaceBefore=12,
    fontName='Helvetica-Bold'
)

subheading_style = ParagraphStyle(
    'CustomSubHeading',
    parent=styles['Heading3'],
    fontSize=11,
    textColor=colors.HexColor('#667eea'),
    spaceAfter=8,
    fontName='Helvetica-Bold'
)

body_style = ParagraphStyle(
    'CustomBody',
    parent=styles['BodyText'],
    fontSize=10,
    alignment=TA_JUSTIFY,
    spaceAfter=8,
    leading=12
)

# ==================== TITLE PAGE ====================
elements.append(Spacer(1, 1.5*inch))
elements.append(Paragraph("CONTACT MANAGEMENT SYSTEM", title_style))
elements.append(Spacer(1, 0.3*inch))
elements.append(Paragraph("User Manual & Reference Guide", styles['Heading3']))
elements.append(Spacer(1, 0.5*inch))
elements.append(Paragraph(f"Version 1.0 | {datetime.now().strftime('%B %d, %Y')}", styles['Normal']))
elements.append(Spacer(1, 2*inch))

info_data = [
    ['Application Type:', 'ASP.NET Core Web Application'],
    ['Database:', 'SQL Server'],
    ['Technology Stack:', 'C#, Entity Framework Core, Bootstrap 5'],
    ['Status:', 'Production Ready'],
]
info_table = Table(info_data, colWidths=[2*inch, 3.5*inch])
info_table.setStyle(TableStyle([
    ('BACKGROUND', (0, 0), (0, -1), colors.HexColor('#f0f4ff')),
    ('TEXTCOLOR', (0, 0), (-1, -1), colors.black),
    ('ALIGN', (0, 0), (-1, -1), 'LEFT'),
    ('FONTNAME', (0, 0), (0, -1), 'Helvetica-Bold'),
    ('FONTSIZE', (0, 0), (-1, -1), 10),
    ('BOTTOMPADDING', (0, 0), (-1, -1), 8),
    ('TOPPADDING', (0, 0), (-1, -1), 8),
    ('GRID', (0, 0), (-1, -1), 1, colors.grey),
]))
elements.append(info_table)

elements.append(PageBreak())

# ==================== TABLE OF CONTENTS ====================
elements.append(Paragraph("TABLE OF CONTENTS", heading_style))
elements.append(Spacer(1, 0.2*inch))

toc_items = [
    "1. Getting Started",
    "2. Login & Authentication",
    "3. Main Dashboard",
    "4. Contact Management",
    "   4.1 Viewing All Contacts",
    "   4.2 Creating New Contacts",
    "   4.3 Editing Contacts",
    "   4.4 Deleting Contacts",
    "   4.5 Contact Details",
    "5. Photo Management",
    "6. Document Management",
    "7. Search & Filter",
    "8. Import/Export Features",
    "9. Advanced Features",
    "   9.1 Duplicate Detection",
    "   9.2 Analytics Dashboard",
    "10. Best Practices",
    "11. Troubleshooting & FAQ",
]

for item in toc_items:
    elements.append(Paragraph(item, body_style))
    elements.append(Spacer(1, 0.05*inch))

elements.append(PageBreak())

# ==================== SECTION 1: GETTING STARTED ====================
elements.append(Paragraph("1. GETTING STARTED", heading_style))
elements.append(Spacer(1, 0.1*inch))

elements.append(Paragraph("<b>System Requirements:</b>", subheading_style))
elements.append(Paragraph("""
• Windows 7 or higher / macOS / Linux<br/>
• Microsoft .NET 8.0 Runtime (or .NET SDK)<br/>
• Microsoft SQL Server 2016 or higher / SQL Server LocalDB<br/>
• Modern web browser (Chrome, Firefox, Edge, Safari)<br/>
• Minimum 2GB RAM<br/>
• Stable internet connection (for local network applications)<br/>
""", body_style))

elements.append(Spacer(1, 0.15*inch))
elements.append(Paragraph("<b>Installation:</b>", subheading_style))
elements.append(Paragraph("""
1. Download the Contact Management System installer<br/>
2. Run ContactManagementSystem-Setup.exe<br/>
3. Follow the installation wizard<br/>
4. Select installation directory<br/>
5. Complete installation and launch application<br/>
6. Application starts automatically on http://localhost:5000<br/>
""", body_style))

elements.append(PageBreak())

# ==================== SECTION 2: LOGIN ====================
elements.append(Paragraph("2. LOGIN & AUTHENTICATION", heading_style))
elements.append(Spacer(1, 0.1*inch))

elements.append(Paragraph("<b>Default Login Credentials:</b>", subheading_style))
login_data = [
    ['Account Type', 'Username', 'Password', 'Access Level'],
    ['Administrator', 'admin', 'Admin@123', 'Full Access'],
    ['Standard User', 'user', 'User@123', 'Limited Access'],
]
login_table = Table(login_data, colWidths=[1.3*inch, 1.3*inch, 1.3*inch, 1.6*inch])
login_table.setStyle(TableStyle([
    ('BACKGROUND', (0, 0), (-1, 0), colors.HexColor('#667eea')),
    ('TEXTCOLOR', (0, 0), (-1, 0), colors.whitesmoke),
    ('ALIGN', (0, 0), (-1, -1), 'CENTER'),
    ('FONTNAME', (0, 0), (-1, 0), 'Helvetica-Bold'),
    ('FONTSIZE', (0, 0), (-1, -1), 9),
    ('BOTTOMPADDING', (0, 0), (-1, -1), 8),
    ('TOPPADDING', (0, 0), (-1, -1), 8),
    ('GRID', (0, 0), (-1, -1), 1, colors.black),
    ('ROWBACKGROUNDS', (0, 1), (-1, -1), [colors.white, colors.HexColor('#f0f4ff')]),
]))
elements.append(login_table)

elements.append(Spacer(1, 0.15*inch))
elements.append(Paragraph("<b>How to Login:</b>", subheading_style))
elements.append(Paragraph("""
1. Open web browser and navigate to http://localhost:5000<br/>
2. You will see the Login page<br/>
3. Enter your Username (admin or user)<br/>
4. Enter your Password<br/>
5. Click the "Login" button<br/>
6. Upon successful login, you will be redirected to the Contacts page<br/>
<br/>
<b>Security Notes:</b><br/>
• Change default passwords after first login<br/>
• Do not share login credentials<br/>
• Always logout before closing the browser<br/>
• Use strong, unique passwords<br/>
""", body_style))

elements.append(PageBreak())

# ==================== SECTION 3: MAIN INTERFACE ====================
elements.append(Paragraph("3. MAIN DASHBOARD", heading_style))
elements.append(Spacer(1, 0.1*inch))

elements.append(Paragraph("<b>Navigation Menu:</b>", subheading_style))
elements.append(Paragraph("""
The main navigation menu at the top provides quick access to:<br/>
<br/>
• <b>All Contacts</b> - View and manage all contacts<br/>
• <b>Dashboard</b> - Analytics and statistics<br/>
• <b>Find Duplicates</b> - Identify duplicate contacts<br/>
• <b>Users</b> - Manage user accounts (Admin only)<br/>
• <b>Groups</b> - Manage contact categories<br/>
• <b>Logout</b> - Sign out from the application<br/>
""", body_style))

elements.append(Spacer(1, 0.15*inch))
elements.append(Paragraph("<b>Quick Action Buttons:</b>", subheading_style))
elements.append(Paragraph("""
• <b>Dashboard</b> - View statistics and charts<br/>
• <b>Find Duplicates</b> - Find similar contacts automatically<br/>
• <b>Add New Contact</b> - Create a new contact<br/>
• <b>Import</b> - Bulk import contacts from files<br/>
• <b>Export</b> - Export all contacts to Excel/CSV/PDF<br/>
""", body_style))

elements.append(PageBreak())

# ==================== SECTION 4: CONTACT MANAGEMENT ====================
elements.append(Paragraph("4. CONTACT MANAGEMENT", heading_style))
elements.append(Spacer(1, 0.1*inch))

elements.append(Paragraph("<b>4.1 Viewing All Contacts</b>", subheading_style))
elements.append(Paragraph("""
The Contacts page displays all your contacts in a organized table format:<br/>
<br/>
<b>Columns displayed:</b><br/>
• Photo - Profile picture of the contact<br/>
• Name - Full name of the contact<br/>
• Email - Email address<br/>
• Phone - Primary phone number<br/>
• WhatsApp - WhatsApp number if available<br/>
• City - City/Location<br/>
• Group - Contact category (Family, Friends, Business, etc.)<br/>
• Actions - View, Edit, Delete options<br/>
<br/>
<b>Contacts are sorted by:</b><br/>
• Recently updated contacts appear first<br/>
• You can search by name, email, or phone<br/>
""", body_style))

elements.append(Spacer(1, 0.1*inch))
elements.append(Paragraph("<b>4.2 Creating New Contacts</b>", subheading_style))
elements.append(Paragraph("""
To add a new contact:<br/>
<br/>
1. Click the "Add New Contact" button<br/>
2. Fill in the contact form with following details:<br/>
   • First Name (Required)<br/>
   • Last Name (Required)<br/>
   • Nickname (Optional)<br/>
   • Email address (Optional)<br/>
   • Phone numbers - up to 3 numbers (Optional)<br/>
   • WhatsApp Number (Optional)<br/>
   • Complete Address (Optional)<br/>
   • City, State, Postal Code, Country (Optional)<br/>
   • Group - Assign to a category<br/>
   • Additional Notes (Optional)<br/>
3. Upload a profile photo (Optional but recommended)<br/>
4. Click "Save Contact"<br/>
5. Contact is created and saved to database<br/>
""", body_style))

elements.append(Spacer(1, 0.1*inch))
elements.append(Paragraph("<b>4.3 Editing Contacts</b>", subheading_style))
elements.append(Paragraph("""
To modify existing contact information:<br/>
<br/>
1. Find the contact in the list<br/>
2. Click the "Edit" button (pencil icon)<br/>
3. Update the desired fields<br/>
4. Change profile photo if needed<br/>
5. Click "Update Contact"<br/>
6. Changes are saved immediately<br/>
""", body_style))

elements.append(Spacer(1, 0.1*inch))
elements.append(Paragraph("<b>4.4 Deleting Contacts</b>", subheading_style))
elements.append(Paragraph("""
To remove a contact:<br/>
<br/>
1. Find the contact in the list<br/>
2. Click the "Delete" button (trash icon)<br/>
3. Confirm deletion when prompted<br/>
4. Contact and all associated data is permanently removed<br/>
<br/>
<b>Note:</b> Deletion is permanent. All photos and documents attached to the contact will also be deleted.<br/>
""", body_style))

elements.append(PageBreak())

# ==================== SECTION 5: PHOTO MANAGEMENT ====================
elements.append(Paragraph("5. PHOTO MANAGEMENT", heading_style))
elements.append(Spacer(1, 0.1*inch))

elements.append(Paragraph("<b>Uploading Photos:</b>", subheading_style))
elements.append(Paragraph("""
For each contact, you can upload multiple photos:<br/>
<br/>
1. Open contact Details page<br/>
2. Scroll to "Photo Gallery" section<br/>
3. Click "Upload Photo"<br/>
4. Select image file from your computer<br/>
5. Supported formats: JPG, PNG, GIF<br/>
6. Maximum file size: 5MB per photo<br/>
7. First photo added becomes the profile picture<br/>
<br/>
<b>Viewing Photos:</b><br/>
• Click on any photo to view full size<br/>
• Photos appear as thumbnails in gallery<br/>
• Set any photo as profile picture<br/>
• Delete unwanted photos<br/>
""", body_style))

elements.append(PageBreak())

# ==================== SECTION 6: DOCUMENT MANAGEMENT ====================
elements.append(Paragraph("6. DOCUMENT MANAGEMENT", heading_style))
elements.append(Spacer(1, 0.1*inch))

elements.append(Paragraph("<b>Attaching Documents:</b>", subheading_style))
elements.append(Paragraph("""
Store important documents with each contact:<br/>
<br/>
1. Open contact Details page<br/>
2. Scroll to "Documents" section<br/>
3. Click "Upload Document"<br/>
4. Select file from your computer<br/>
5. Add document type (ID, Address Proof, Contract, Certificate, etc.)<br/>
6. Supported formats: PDF, DOC, DOCX, XLS, XLSX, PPT, PPTX<br/>
7. Maximum file size: 10MB per document<br/>
<br/>
<b>Managing Documents:</b><br/>
• View uploaded documents<br/>
• Download documents to your computer<br/>
• Delete unwanted documents<br/>
• Documents remain secure in the system<br/>
""", body_style))

elements.append(PageBreak())

# ==================== SECTION 7: SEARCH & FILTER ====================
elements.append(Paragraph("7. SEARCH & FILTER", heading_style))
elements.append(Spacer(1, 0.1*inch))

elements.append(Paragraph("<b>Searching Contacts:</b>", subheading_style))
elements.append(Paragraph("""
To find specific contacts quickly:<br/>
<br/>
1. On the Contacts page, use the search box<br/>
2. Enter search term:<br/>
   • Contact name (First or Last)<br/>
   • Email address<br/>
   • Phone number<br/>
3. Press Enter or click "Search" button<br/>
4. Results display matching contacts<br/>
5. Click "Clear" to reset and view all contacts<br/>
<br/>
<b>Search Examples:</b><br/>
• Search "John" - finds all contacts with First Name "John"<br/>
• Search "john.doe@example.com" - finds by email<br/>
• Search "+1-555-0101" - finds by phone number<br/>
""", body_style))

elements.append(PageBreak())

# ==================== SECTION 8: IMPORT/EXPORT ====================
elements.append(Paragraph("8. IMPORT/EXPORT FEATURES", heading_style))
elements.append(Spacer(1, 0.1*inch))

elements.append(Paragraph("<b>8.1 Importing Contacts</b>", subheading_style))
elements.append(Paragraph("""
Bulk import contacts from Excel or CSV files:<br/>
<br/>
1. Click "Import" button on Contacts page<br/>
2. Download import template (Excel or CSV)<br/>
3. Fill template with your contact data:<br/>
   • FirstName, LastName (Required)<br/>
   • NickName, Email, Mobile1, Mobile2, Mobile3<br/>
   • WhatsAppNumber, Address, City, State<br/>
   • PostalCode, Country, OtherDetails<br/>
4. Select file type (Excel or CSV)<br/>
5. Choose file from your computer<br/>
6. Click "Import Contacts"<br/>
7. System processes and adds new contacts<br/>
8. View import results and confirmation<br/>
<br/>
<b>Important:</b> Import adds new contacts. It does not update existing ones.<br/>
""", body_style))

elements.append(Spacer(1, 0.1*inch))
elements.append(Paragraph("<b>8.2 Exporting Contacts</b>", subheading_style))
elements.append(Paragraph("""
Export all contacts to various formats:<br/>
<br/>
1. Click "Export" dropdown button<br/>
2. Choose export format:<br/>
   • <b>Excel</b> - .xlsx format with formatting<br/>
   • <b>CSV</b> - Comma-separated values<br/>
   • <b>PDF</b> - Professional formatted document<br/>
3. File downloads automatically to your computer<br/>
4. Use exported data for backup or sharing<br/>
""", body_style))

elements.append(PageBreak())

# ==================== SECTION 9: ADVANCED FEATURES ====================
elements.append(Paragraph("9. ADVANCED FEATURES", heading_style))
elements.append(Spacer(1, 0.1*inch))

elements.append(Paragraph("<b>9.1 Duplicate Detection</b>", subheading_style))
elements.append(Paragraph("""
Automatically find duplicate or similar contacts:<br/>
<br/>
1. Click "Find Duplicates" button<br/>
2. System scans all contacts<br/>
3. Compares names, emails, and phone numbers<br/>
4. Displays potential duplicates with similarity percentage<br/>
5. Review each pair side-by-side<br/>
6. Click to view or edit suspected duplicates<br/>
7. Manually merge or keep duplicates as needed<br/>
<br/>
<b>Similarity Algorithm:</b><br/>
Uses Levenshtein distance algorithm<br/>
70% similarity threshold<br/>
Weighted scoring for emails and phones<br/>
""", body_style))

elements.append(Spacer(1, 0.1*inch))
elements.append(Paragraph("<b>9.2 Analytics Dashboard</b>", subheading_style))
elements.append(Paragraph("""
View statistics and insights about your contacts:<br/>
<br/>
<b>Dashboard metrics:</b><br/>
• Total number of contacts<br/>
• Email coverage percentage<br/>
• Phone coverage percentage<br/>
• Top 5 cities by contact count<br/>
• Contacts by group distribution<br/>
• Auto-refreshes every 5 minutes<br/>
<br/>
<b>Dashboard layout (updated):</b><br/>
• Four KPI cards in a single row with centered values<br/>
• Two charts side-by-side (Contacts by Group, Top Cities)<br/>
• Compact single-screen layout with no vertical scrolling<br/>
• Cards and charts optimized for clarity and fast scanning<br/>
<br/>
Use dashboard to understand your contact database<br/>
and identify data gaps.<br/>
""", body_style))

elements.append(PageBreak())

# ==================== SECTION 10: BEST PRACTICES ====================
elements.append(Paragraph("10. BEST PRACTICES", heading_style))
elements.append(Spacer(1, 0.1*inch))

elements.append(Paragraph("""
<b>Data Organization:</b><br/>
• Use consistent naming conventions<br/>
• Assign contacts to appropriate groups<br/>
• Keep contact information up-to-date<br/>
• Add profile photos for visual recognition<br/>
• Include complete address information<br/>
<br/>
<b>Regular Maintenance:</b><br/>
• Review and update contacts regularly<br/>
• Use duplicate detection periodically<br/>
• Remove obsolete contacts<br/>
• Verify phone numbers and emails<br/>
<br/>
<b>Data Security:</b><br/>
• Change default passwords immediately<br/>
• Use strong, unique passwords<br/>
• Logout before closing application<br/>
• Regular backups of database<br/>
• Limit access to sensitive information<br/>
<br/>
<b>Efficient Usage:</b><br/>
• Use search for quick lookup<br/>
• Organize contacts into groups<br/>
• Use import for bulk additions<br/>
• Export regularly for backup<br/>
• Check analytics to monitor coverage<br/>
""", body_style))

elements.append(PageBreak())

# ==================== SECTION 11: TROUBLESHOOTING ====================
elements.append(Paragraph("11. TROUBLESHOOTING & FAQ", heading_style))
elements.append(Spacer(1, 0.1*inch))

elements.append(Paragraph("<b>Frequently Asked Questions:</b>", subheading_style))
elements.append(Paragraph("""
<b>Q: How do I reset my password?</b><br/>
A: Contact your administrator to reset your password. Administrators can reset user passwords from the Users management page.<br/>
<br/>
<b>Q: Can I change the default admin password?</b><br/>
A: Yes, you can change it. However, you need to edit the database directly or contact your system administrator.<br/>
<br/>
<b>Q: How do I backup my contacts?</b><br/>
A: Use the Export feature to save all contacts to Excel, CSV, or PDF format. Keep these files as backup copies.<br/>
<br/>
<b>Q: Can I import contacts from my phone?</b><br/>
A: Yes! Export contacts from your phone as CSV, then use the Import feature to add them.<br/>
<br/>
<b>Q: What file formats are supported?</b><br/>
Photos: JPG, PNG, GIF<br/>
Documents: PDF, DOC, DOCX, XLS, XLSX, PPT<br/>
Import: Excel (.xlsx), CSV<br/>
<br/>
<b>Q: How do I delete all contacts?</b><br/>
A: Delete contacts individually from the list. There is no bulk delete feature for safety.<br/>
<br/>
<b>Q: Can multiple users access the system?</b><br/>
A: Yes! Each user has their own login credentials and access level determined by their user group.<br/>
<br/>
<b>Q: Is the system cloud-based?</b><br/>
A: This version is a local application. It runs on your computer or local network server.<br/>
""", body_style))

elements.append(Spacer(1, 0.2*inch))
elements.append(Paragraph("<b>Common Issues & Solutions:</b>", subheading_style))
elements.append(Paragraph("""
<b>Issue: Cannot login</b><br/>
Solution: Verify username and password are correct. Check caps lock. Clear browser cache.<br/>
<br/>
<b>Issue: Application not responding</b><br/>
Solution: Refresh the page. Restart the application. Check your internet connection.<br/>
<br/>
<b>Issue: Photo not uploading</b><br/>
Solution: Check file size (max 5MB). Verify format (JPG, PNG, GIF). Try another browser.<br/>
<br/>
<b>Issue: Import failing</b><br/>
Solution: Verify Excel/CSV format matches template. Check for duplicate entries. Ensure required fields are filled.<br/>
<br/>
<b>Issue: Search not working</b><br/>
Solution: Clear search box completely. Check spelling. Try searching by exact match first.<br/>
<br/>
<b>Issue: Database locked error</b><br/>
Solution: Close other instances of the application. Restart the service. Check file permissions.<br/>
""", body_style))

elements.append(PageBreak())

# ==================== SUPPORT & CONTACT ====================
elements.append(Paragraph("SUPPORT & CONTACT INFORMATION", heading_style))
elements.append(Spacer(1, 0.1*inch))

elements.append(Paragraph("""
<b>Getting Help:</b><br/>
• Review this user manual<br/>
• Check the FAQ section above<br/>
• Contact your system administrator<br/>
• Review application error messages<br/>
<br/>
<b>Reporting Issues:</b><br/>
• Document the problem clearly<br/>
• Note any error messages<br/>
• Provide steps to reproduce the issue<br/>
• Include screenshots if possible<br/>
<br/>
<b>Feature Requests:</b><br/>
• Contact your administrator<br/>
• Suggest improvements<br/>
• Request additional features<br/>
<br/>
<b>Version Information:</b><br/>
• Application Version: 1.0<br/>
• Created: February 2026<br/>
• Platform: ASP.NET Core 8.0<br/>
• Database: SQL Server<br/>
""", body_style))

elements.append(Spacer(1, 0.5*inch))
elements.append(Paragraph("Thank you for using Contact Management System!", styles['Heading2']))
elements.append(Spacer(1, 0.1*inch))
elements.append(Paragraph(f"Manual Generated: {datetime.now().strftime('%B %d, %Y at %I:%M %p')}", styles['Normal']))

# Build PDF
doc.build(elements)
print(f"✓ User Manual PDF created successfully: {pdf_file}")
print(f"✓ Location: e:\\Contact_Management_System\\{pdf_file}")
