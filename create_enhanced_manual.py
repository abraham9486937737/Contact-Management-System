#!/usr/bin/env python3
"""
Create Enhanced User Manual PDF with Database Schema & Screenshot Documentation
"""

from reportlab.lib.pagesizes import letter
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib.units import inch
from reportlab.lib.enums import TA_CENTER, TA_LEFT, TA_JUSTIFY, TA_RIGHT
from reportlab.platypus import SimpleDocTemplate, Paragraph, Spacer, PageBreak, Table, TableStyle, Image
from reportlab.lib import colors
from datetime import datetime

# Create PDF
pdf_file = "Contact_Management_System_Complete_Manual.pdf"
doc = SimpleDocTemplate(pdf_file, pagesize=letter, topMargin=0.5*inch, bottomMargin=0.5*inch)

elements = []
styles = getSampleStyleSheet()

# Custom styles
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
    fontSize=9.5,
    alignment=TA_JUSTIFY,
    spaceAfter=6,
    leading=11
)

code_style = ParagraphStyle(
    'CodeStyle',
    parent=styles['BodyText'],
    fontSize=8,
    fontName='Courier',
    textColor=colors.HexColor('#333333'),
    leftIndent=20,
    spaceAfter=6,
    leading=10
)

# ==================== TITLE PAGE ====================
elements.append(Spacer(1, 1.5*inch))
elements.append(Paragraph("CONTACT MANAGEMENT SYSTEM", title_style))
elements.append(Spacer(1, 0.2*inch))
elements.append(Paragraph("Complete User Manual with Database Schema", styles['Heading3']))
elements.append(Spacer(1, 0.1*inch))
elements.append(Paragraph("& Technical Documentation", styles['Heading3']))
elements.append(Spacer(1, 0.5*inch))
elements.append(Paragraph(f"Version 1.0 (Enhanced) | {datetime.now().strftime('%B %d, %Y')}", styles['Normal']))
elements.append(Spacer(1, 1.5*inch))

info_data = [
    ['Framework:', 'ASP.NET Core 8.0 MVC'],
    ['Language:', 'C#'],
    ['Database:', 'Microsoft SQL Server'],
    ['ORM:', 'Entity Framework Core'],
    ['Frontend:', 'HTML5, Bootstrap 5, JavaScript'],
    ['Status:', 'Production Ready'],
]
info_table = Table(info_data, colWidths=[2*inch, 3.5*inch])
info_table.setStyle(TableStyle([
    ('BACKGROUND', (0, 0), (0, -1), colors.HexColor('#f0f4ff')),
    ('TEXTCOLOR', (0, 0), (-1, -1), colors.black),
    ('ALIGN', (0, 0), (-1, -1), 'LEFT'),
    ('FONTNAME', (0, 0), (0, -1), 'Helvetica-Bold'),
    ('FONTSIZE', (0, 0), (-1, -1), 9),
    ('BOTTOMPADDING', (0, 0), (-1, -1), 8),
    ('TOPPADDING', (0, 0), (-1, -1), 8),
    ('GRID', (0, 0), (-1, -1), 1, colors.grey),
]))
elements.append(info_table)

elements.append(PageBreak())

# ==================== TABLE OF CONTENTS ====================
elements.append(Paragraph("TABLE OF CONTENTS", heading_style))
elements.append(Spacer(1, 0.15*inch))

toc_items = [
    "1. Introduction & Quick Start",
    "2. System Architecture",
    "3. Database Schema & Structure",
    "   3.1 Tables Overview",
    "   3.2 Detailed Table Schemas",
    "   3.3 Entity Relationships",
    "4. User Interface Guide",
    "   4.1 Screenshots & Walkthrough",
    "5. Feature Documentation",
    "6. Import/Export Specifications",
    "7. Best Practices",
    "8. Troubleshooting",
    "9. Technical Support",
]

for item in toc_items:
    elements.append(Paragraph(item, body_style))
    elements.append(Spacer(1, 0.04*inch))

elements.append(PageBreak())

# ==================== SYSTEM ARCHITECTURE ====================
elements.append(Paragraph("2. SYSTEM ARCHITECTURE", heading_style))
elements.append(Spacer(1, 0.1*inch))

elements.append(Paragraph("<b>Technology Stack:</b>", subheading_style))
elements.append(Paragraph("""
<b>Frontend Layer:</b><br/>
• ASP.NET Core Razor Pages & MVC Controllers<br/>
• Bootstrap 5 for responsive UI design<br/>
• Font Awesome for icons<br/>
• JavaScript for interactivity<br/>
<br/>
<b>Business Logic Layer:</b><br/>
• C# Service classes for business operations<br/>
• ImportExportService - Handle file operations<br/>
• ContactStatisticsService - Analytics & duplicates<br/>
• FileUploadService - Media file management<br/>
<br/>
<b>Data Access Layer:</b><br/>
• Entity Framework Core 8.0<br/>
• LINQ-to-SQL queries<br/>
• Database migrations for version control<br/>
<br/>
<b>Database Layer:</b><br/>
• Microsoft SQL Server (Production)<br/>
• SQL Server LocalDB (Development)<br/>
• Normalized relational database design<br/>
""", body_style))

elements.append(PageBreak())

# ==================== DATABASE SCHEMA ====================
elements.append(Paragraph("3. DATABASE SCHEMA & STRUCTURE", heading_style))
elements.append(Spacer(1, 0.1*inch))

elements.append(Paragraph("<b>3.1 Database Tables Overview</b>", subheading_style))

tables_overview = [
    ['Table Name', 'Purpose', 'Records', 'Key Relationships'],
    ['Contacts', 'Store contact details', 'Multiple', 'FK to ContactGroups, Photos, Documents'],
    ['ContactPhotos', 'Store contact photos', 'Multiple per contact', 'FK to Contacts'],
    ['ContactDocuments', 'Store contact documents', 'Multiple per contact', 'FK to Contacts'],
    ['ContactGroups', 'Store contact categories', 'Multiple', 'Referenced by Contacts'],
    ['AppUsers', 'Store user accounts', 'Multiple', 'FK to UserGroups, Rights'],
    ['UserGroups', 'Store user role groups', 'Multiple', 'Referenced by AppUsers, Rights'],
    ['GroupRights', 'Store group permissions', 'Multiple', 'FK to UserGroups'],
    ['UserRights', 'Store individual perms', 'Multiple', 'FK to AppUsers'],
]

tables_table = Table(tables_overview, colWidths=[1.2*inch, 1.8*inch, 1*inch, 1.7*inch])
tables_table.setStyle(TableStyle([
    ('BACKGROUND', (0, 0), (-1, 0), colors.HexColor('#667eea')),
    ('TEXTCOLOR', (0, 0), (-1, 0), colors.whitesmoke),
    ('ALIGN', (0, 0), (-1, 0), 'CENTER'),
    ('ALIGN', (0, 1), (-1, -1), 'LEFT'),
    ('FONTNAME', (0, 0), (-1, 0), 'Helvetica-Bold'),
    ('FONTSIZE', (0, 0), (-1, -1), 8),
    ('BOTTOMPADDING', (0, 0), (-1, -1), 6),
    ('TOPPADDING', (0, 0), (-1, -1), 6),
    ('GRID', (0, 0), (-1, -1), 1, colors.black),
    ('ROWBACKGROUNDS', (0, 1), (-1, -1), [colors.white, colors.HexColor('#f0f4ff')]),
]))
elements.append(tables_table)

elements.append(PageBreak())

# ==================== DETAILED TABLE SCHEMAS ====================
elements.append(Paragraph("<b>3.2 Detailed Table Schemas</b>", subheading_style))
elements.append(Spacer(1, 0.1*inch))

# Contact Table
elements.append(Paragraph("<b>📋 CONTACTS Table</b>", subheading_style))
contact_cols = [
    ['Column Name', 'Data Type', 'Constraints', 'Description'],
    ['Id', 'INT', 'PK, Identity', 'Primary key auto-increment'],
    ['FirstName', 'NVARCHAR(100)', 'NOT NULL', 'Contact first name (Required)'],
    ['LastName', 'NVARCHAR(100)', 'NULL', 'Contact last name (Optional)'],
    ['NickName', 'NVARCHAR(100)', 'NULL', 'Contact nickname'],
    ['Email', 'NVARCHAR(255)', 'NULL', 'Email address (Indexed)'],
    ['Mobile1', 'NVARCHAR(20)', 'NULL', 'Primary phone number'],
    ['Mobile2', 'NVARCHAR(20)', 'NULL', 'Secondary phone number'],
    ['Mobile3', 'NVARCHAR(20)', 'NULL', 'Tertiary phone number'],
    ['WhatsAppNumber', 'NVARCHAR(20)', 'NULL', 'WhatsApp contact number'],
    ['Address', 'NVARCHAR(250)', 'NULL', 'Street address'],
    ['City', 'NVARCHAR(100)', 'NULL', 'City/Location'],
    ['State', 'NVARCHAR(100)', 'NULL', 'State/Province'],
    ['PostalCode', 'NVARCHAR(20)', 'NULL', 'ZIP/Postal code'],
    ['Country', 'NVARCHAR(100)', 'NULL', 'Country name'],
    ['PhotoPath', 'NVARCHAR(500)', 'NULL', 'Path to profile photo'],
    ['GroupId', 'INT', 'FK(ContactGroups)', 'Reference to group'],
    ['OtherDetails', 'NVARCHAR(MAX)', 'NULL', 'Additional notes/details'],
    ['CreatedAt', 'DATETIME2', 'NOT NULL', 'Creation timestamp'],
    ['UpdatedAt', 'DATETIME2', 'NOT NULL', 'Last update timestamp'],
]

contact_table = Table(contact_cols, colWidths=[1.1*inch, 1.1*inch, 1.2*inch, 1.4*inch])
contact_table.setStyle(TableStyle([
    ('BACKGROUND', (0, 0), (-1, 0), colors.HexColor('#667eea')),
    ('TEXTCOLOR', (0, 0), (-1, 0), colors.whitesmoke),
    ('ALIGN', (0, 0), (-1, -1), 'LEFT'),
    ('FONTNAME', (0, 0), (-1, 0), 'Helvetica-Bold'),
    ('FONTSIZE', (0, 0), (-1, -1), 7),
    ('BOTTOMPADDING', (0, 0), (-1, -1), 4),
    ('TOPPADDING', (0, 0), (-1, -1), 4),
    ('GRID', (0, 0), (-1, -1), 1, colors.grey),
    ('ROWBACKGROUNDS', (0, 1), (-1, -1), [colors.white, colors.HexColor('#f9f9ff')]),
]))
elements.append(contact_table)
elements.append(Spacer(1, 0.12*inch))

# ContactPhotos Table
elements.append(Paragraph("<b>📸 CONTACT_PHOTOS Table</b>", subheading_style))
photos_cols = [
    ['Column Name', 'Data Type', 'Constraints', 'Description'],
    ['Id', 'INT', 'PK, Identity', 'Primary key auto-increment'],
    ['ContactId', 'INT', 'FK(Contacts)', 'Reference to Contact'],
    ['PhotoPath', 'NVARCHAR(500)', 'NOT NULL', 'Path to photo file'],
    ['FileName', 'NVARCHAR(255)', 'NOT NULL', 'Original filename'],
    ['FileSize', 'BIGINT', 'NOT NULL', 'File size in bytes'],
    ['ContentType', 'NVARCHAR(50)', 'NOT NULL', 'MIME type (image/jpeg)'],
    ['IsProfilePhoto', 'BIT', 'NOT NULL', 'Flag for profile picture'],
    ['UploadedAt', 'DATETIME2', 'NOT NULL', 'Upload timestamp'],
]

photos_table = Table(photos_cols, colWidths=[1.1*inch, 1.1*inch, 1.2*inch, 1.4*inch])
photos_table.setStyle(TableStyle([
    ('BACKGROUND', (0, 0), (-1, 0), colors.HexColor('#667eea')),
    ('TEXTCOLOR', (0, 0), (-1, 0), colors.whitesmoke),
    ('ALIGN', (0, 0), (-1, -1), 'LEFT'),
    ('FONTNAME', (0, 0), (-1, 0), 'Helvetica-Bold'),
    ('FONTSIZE', (0, 0), (-1, -1), 7),
    ('BOTTOMPADDING', (0, 0), (-1, -1), 4),
    ('TOPPADDING', (0, 0), (-1, -1), 4),
    ('GRID', (0, 0), (-1, -1), 1, colors.grey),
    ('ROWBACKGROUNDS', (0, 1), (-1, -1), [colors.white, colors.HexColor('#f9f9ff')]),
]))
elements.append(photos_table)
elements.append(Spacer(1, 0.12*inch))

# ContactDocuments Table
elements.append(Paragraph("<b>📄 CONTACT_DOCUMENTS Table</b>", subheading_style))
docs_cols = [
    ['Column Name', 'Data Type', 'Constraints', 'Description'],
    ['Id', 'INT', 'PK, Identity', 'Primary key auto-increment'],
    ['ContactId', 'INT', 'FK(Contacts)', 'Reference to Contact'],
    ['DocumentPath', 'NVARCHAR(500)', 'NOT NULL', 'Path to document file'],
    ['FileName', 'NVARCHAR(255)', 'NOT NULL', 'Original filename'],
    ['FileSize', 'BIGINT', 'NOT NULL', 'File size in bytes'],
    ['ContentType', 'NVARCHAR(50)', 'NOT NULL', 'MIME type'],
    ['DocumentType', 'NVARCHAR(100)', 'NOT NULL', 'Type (ID, Address, etc)'],
    ['UploadedAt', 'DATETIME2', 'NOT NULL', 'Upload timestamp'],
]

docs_table = Table(docs_cols, colWidths=[1.1*inch, 1.1*inch, 1.2*inch, 1.4*inch])
docs_table.setStyle(TableStyle([
    ('BACKGROUND', (0, 0), (-1, 0), colors.HexColor('#667eea')),
    ('TEXTCOLOR', (0, 0), (-1, 0), colors.whitesmoke),
    ('ALIGN', (0, 0), (-1, -1), 'LEFT'),
    ('FONTNAME', (0, 0), (-1, 0), 'Helvetica-Bold'),
    ('FONTSIZE', (0, 0), (-1, -1), 7),
    ('BOTTOMPADDING', (0, 0), (-1, -1), 4),
    ('TOPPADDING', (0, 0), (-1, -1), 4),
    ('GRID', (0, 0), (-1, -1), 1, colors.grey),
    ('ROWBACKGROUNDS', (0, 1), (-1, -1), [colors.white, colors.HexColor('#f9f9ff')]),
]))
elements.append(docs_table)

elements.append(PageBreak())

# User Management Tables
elements.append(Paragraph("<b>👤 APP_USERS Table</b>", subheading_style))
users_cols = [
    ['Column Name', 'Data Type', 'Constraints', 'Description'],
    ['Id', 'INT', 'PK, Identity', 'Primary key auto-increment'],
    ['UserName', 'NVARCHAR(100)', 'NOT NULL, Unique', 'Login username (Indexed)'],
    ['PasswordHash', 'NVARCHAR(MAX)', 'NOT NULL', 'Hashed password'],
    ['FullName', 'NVARCHAR(200)', 'NULL', 'User full name'],
    ['IsAdmin', 'BIT', 'NOT NULL', 'Administrator flag'],
    ['IsActive', 'BIT', 'NOT NULL', 'Account active status'],
    ['GroupId', 'INT', 'FK(UserGroups)', 'Reference to user group'],
    ['CreatedAt', 'DATETIME2', 'NOT NULL', 'Creation timestamp'],
    ['UpdatedAt', 'DATETIME2', 'NOT NULL', 'Last update timestamp'],
]

users_table = Table(users_cols, colWidths=[1.1*inch, 1.1*inch, 1.2*inch, 1.4*inch])
users_table.setStyle(TableStyle([
    ('BACKGROUND', (0, 0), (-1, 0), colors.HexColor('#764ba2')),
    ('TEXTCOLOR', (0, 0), (-1, 0), colors.whitesmoke),
    ('ALIGN', (0, 0), (-1, -1), 'LEFT'),
    ('FONTNAME', (0, 0), (-1, 0), 'Helvetica-Bold'),
    ('FONTSIZE', (0, 0), (-1, -1), 7),
    ('BOTTOMPADDING', (0, 0), (-1, -1), 4),
    ('TOPPADDING', (0, 0), (-1, -1), 4),
    ('GRID', (0, 0), (-1, -1), 1, colors.grey),
    ('ROWBACKGROUNDS', (0, 1), (-1, -1), [colors.white, colors.HexColor('#f9f9ff')]),
]))
elements.append(users_table)
elements.append(Spacer(1, 0.12*inch))

# UserGroups Table
elements.append(Paragraph("<b>🔐 USER_GROUPS Table</b>", subheading_style))
groups_cols = [
    ['Column Name', 'Data Type', 'Constraints', 'Description'],
    ['Id', 'INT', 'PK, Identity', 'Primary key auto-increment'],
    ['Name', 'NVARCHAR(150)', 'NOT NULL, Unique', 'Group name (Admin, User, etc)'],
    ['Description', 'NVARCHAR(500)', 'NULL', 'Group description'],
    ['CreatedAt', 'DATETIME2', 'NOT NULL', 'Creation timestamp'],
]

groups_table = Table(groups_cols, colWidths=[1.1*inch, 1.1*inch, 1.2*inch, 1.4*inch])
groups_table.setStyle(TableStyle([
    ('BACKGROUND', (0, 0), (-1, 0), colors.HexColor('#764ba2')),
    ('TEXTCOLOR', (0, 0), (-1, 0), colors.whitesmoke),
    ('ALIGN', (0, 0), (-1, -1), 'LEFT'),
    ('FONTNAME', (0, 0), (-1, 0), 'Helvetica-Bold'),
    ('FONTSIZE', (0, 0), (-1, -1), 7),
    ('BOTTOMPADDING', (0, 0), (-1, -1), 4),
    ('TOPPADDING', (0, 0), (-1, -1), 4),
    ('GRID', (0, 0), (-1, -1), 1, colors.grey),
    ('ROWBACKGROUNDS', (0, 1), (-1, -1), [colors.white, colors.HexColor('#f9f9ff')]),
]))
elements.append(groups_table)

elements.append(PageBreak())

# Entity Relationships
elements.append(Paragraph("<b>3.3 Entity Relationship Diagram (ERD)</b>", subheading_style))
elements.append(Spacer(1, 0.1*inch))

erd_data = [
    Paragraph("""<b>Database Relationships:</b><br/><br/>
1. <b>Contacts ↔ ContactPhotos (1:N)</b><br/>
   • One contact has many photos<br/>
   • Delete cascade: Deleting contact removes all photos<br/>
   • Foreign Key: ContactPhotos.ContactId → Contacts.Id<br/>
<br/>
2. <b>Contacts ↔ ContactDocuments (1:N)</b><br/>
   • One contact has many documents<br/>
   • Delete cascade: Deleting contact removes all documents<br/>
   • Foreign Key: ContactDocuments.ContactId → Contacts.Id<br/>
<br/>
3. <b>Contacts ↔ ContactGroups (N:1)</b><br/>
   • Many contacts belong to one group<br/>
   • Set null on delete: Group deletion nullifies contact group<br/>
   • Foreign Key: Contacts.GroupId → ContactGroups.Id<br/>
<br/>
4. <b>AppUsers ↔ UserGroups (N:1)</b><br/>
   • Many users belong to one group<br/>
   • Set null on delete<br/>
   • Foreign Key: AppUsers.GroupId → UserGroups.Id<br/>
<br/>
5. <b>AppUsers ↔ UserRights (1:N)</b><br/>
   • One user has many rights<br/>
   • Delete cascade<br/>
   • Foreign Key: UserRights.AppUserId → AppUsers.Id<br/>
<br/>
6. <b>UserGroups ↔ GroupRights (1:N)</b><br/>
   • One group has many rights<br/>
   • Delete cascade<br/>
   • Foreign Key: GroupRights.UserGroupId → UserGroups.Id<br/>
""", body_style)
]

elements.extend(erd_data)

elements.append(PageBreak())

# ==================== USER INTERFACE SECTION ====================
elements.append(Paragraph("4. USER INTERFACE GUIDE", heading_style))
elements.append(Spacer(1, 0.1*inch))

elements.append(Paragraph("<b>4.1 Screenshots & Walkthrough</b>", subheading_style))
elements.append(Spacer(1, 0.1*inch))

screenshots_info = Paragraph("""
📸 <b>SCREENSHOT INTEGRATION GUIDE:</b><br/>
<br/>
To add screenshots to this document, follow these steps:<br/>
<br/>
1. <b>Capture Screenshots:</b><br/>
   • Login page: http://localhost:5000/account/login<br/>
   • Contacts list: http://localhost:5000/home/index<br/>
   • Create contact form: http://localhost:5000/home/create<br/>
   • Contact details: Click any contact in the list<br/>
   • Import page: Click Import button<br/>
   • Dashboard: Click Dashboard button<br/>
   • Find Duplicates: Click Find Duplicates button<br/>
    • Dashboard layout: 4 KPI cards in one row, 2 charts side-by-side<br/>
<br/>
2. <b>Screenshot Specifications:</b><br/>
   • Format: PNG or JPG<br/>
   • Resolution: 1280x720 or higher<br/>
   • File size: Less than 2MB each<br/>
   • Background: Light/white for clarity<br/>
<br/>
3. <b>How to Add to PDF:</b><br/>
   • Save screenshots in: e:\\Contact_Management_System\\screenshots\\<br/>
   • Modify the Python script to insert images<br/>
   • Use reportlab: Image('screenshot.png', width=5.5*inch, height=3*inch)<br/>
<br/>
4. <b>Screenshot Checklist:</b><br/>
   ☐ Login page - Show credential entry<br/>
   ☐ Dashboard/Home - Show contacts list<br/>
   ☐ Create Contact - Show form fields<br/>
   ☐ Contact Details - Show complete information<br/>
   ☐ Import Page - Show import interface<br/>
   ☐ Export Options - Show export dropdown<br/>
   ☐ Photo Gallery - Show photo management<br/>
   ☐ Documents Section - Show document management<br/>
   ☐ Search Function - Show search results<br/>
   ☐ Analytics Dashboard - Show statistics<br/>
""", body_style)

elements.append(screenshots_info)

elements.append(PageBreak())

# ==================== DATABASE SPECIFICATIONS ====================
elements.append(Paragraph("DATABASE SPECIFICATIONS & INDEXES", heading_style))
elements.append(Spacer(1, 0.1*inch))

elements.append(Paragraph("<b>Indexes for Performance:</b>", subheading_style))
elements.append(Paragraph("""
<b>Primary Keys (Clustered Indexes):</b><br/>
• Contacts.Id<br/>
• ContactPhotos.Id<br/>
• ContactDocuments.Id<br/>
• ContactGroups.Id<br/>
• AppUsers.Id<br/>
• UserGroups.Id<br/>
• UserRights.Id<br/>
• GroupRights.Id<br/>
<br/>
<b>Non-Clustered Indexes (for search performance):</b><br/>
• Contacts.Email (for email lookup)<br/>
• Contacts.FirstName, Contacts.LastName (for name search)<br/>
• AppUsers.UserName (Unique - for login)<br/>
• ContactPhotos.ContactId (Foreign Key)<br/>
• ContactDocuments.ContactId (Foreign Key)<br/>
• GroupRights (Composite: UserGroupId + RightKey)<br/>
• UserRights (Composite: AppUserId + RightKey)<br/>
<br/>
<b>Unique Constraints:</b><br/>
• AppUsers.UserName (One login per user)<br/>
• UserGroups.Name (Unique group names)<br/>
• GroupRights (UserGroupId + RightKey) combination<br/>
• UserRights (AppUserId + RightKey) combination<br/>
""", body_style))

elements.append(PageBreak())

# ==================== DATA TYPES & CONSTRAINTS ====================
elements.append(Paragraph("DATA TYPES & VALIDATION", heading_style))
elements.append(Spacer(1, 0.1*inch))

elements.append(Paragraph("""
<b>SQL Server Data Types Used:</b><br/><br/>
• <b>NVARCHAR(n)</b> - Unicode text, variable length<br/>
   Used for: Names, emails, addresses, descriptions<br/>
   Max length: 100-500 characters<br/>
<br/>
• <b>INT</b> - 32-bit integer<br/>
   Used for: IDs, counts, foreign keys<br/>
   Range: -2,147,483,648 to 2,147,483,647<br/>
<br/>
• <b>BIGINT</b> - 64-bit integer<br/>
   Used for: File sizes (bytes)<br/>
   Supports: Up to 9.2 × 10^18 bytes<br/>
<br/>
• <b>BIT</b> - Boolean (0 or 1)<br/>
   Used for: Flags (IsAdmin, IsActive, IsProfilePhoto)<br/>
<br/>
• <b>DATETIME2</b> - Date and time with millisecond precision<br/>
   Used for: Timestamps (CreatedAt, UpdatedAt, UploadedAt)<br/>
   Format: YYYY-MM-DD HH:MM:SS.ffffff<br/>
<br/>
• <b>NVARCHAR(MAX)</b> - Unlimited text<br/>
   Used for: OtherDetails, extended descriptions<br/>
   Max size: 2 GB<br/>
<br/>
<b>Field Constraints:</b><br/>
• NOT NULL - Field is mandatory<br/>
• NULL - Field is optional<br/>
• UNIQUE - No duplicate values allowed<br/>
• DEFAULT - Default value if not provided<br/>
• IDENTITY(1,1) - Auto-increment starting at 1<br/>
• PRIMARY KEY - Unique identifier for each row<br/>
• FOREIGN KEY - Reference to another table<br/>
""", body_style))

elements.append(PageBreak())

# ==================== DATA VALIDATION RULES ====================
elements.append(Paragraph("APPLICATION-LEVEL VALIDATION", heading_style))
elements.append(Spacer(1, 0.1*inch))

validation_data = [
    ['Field', 'Validation Rule', 'Error Message'],
    ['FirstName', 'Required, Max 100 chars', 'First Name is required'],
    ['Email', 'Valid email format', 'Invalid email format'],
    ['Mobile1-3', 'Phone format, Max 20 chars', 'Invalid phone number'],
    ['Password', 'Min 8 chars, complex', 'Password must be strong'],
    ['File Upload', 'Max 5MB for photos, 10MB for docs', 'File size exceeds limit'],
    ['Photo Type', 'JPG, PNG, GIF only', 'Unsupported file format'],
    ['Document Type', 'PDF, DOC, DOCX, XLS, XLSX', 'Document format not allowed'],
]

validation_table = Table(validation_data, colWidths=[1.3*inch, 2.5*inch, 1.7*inch])
validation_table.setStyle(TableStyle([
    ('BACKGROUND', (0, 0), (-1, 0), colors.HexColor('#667eea')),
    ('TEXTCOLOR', (0, 0), (-1, 0), colors.whitesmoke),
    ('ALIGN', (0, 0), (-1, -1), 'LEFT'),
    ('FONTNAME', (0, 0), (-1, 0), 'Helvetica-Bold'),
    ('FONTSIZE', (0, 0), (-1, -1), 8),
    ('BOTTOMPADDING', (0, 0), (-1, -1), 6),
    ('TOPPADDING', (0, 0), (-1, -1), 6),
    ('GRID', (0, 0), (-1, -1), 1, colors.black),
    ('ROWBACKGROUNDS', (0, 1), (-1, -1), [colors.white, colors.HexColor('#f0f4ff')]),
]))
elements.append(validation_table)

elements.append(PageBreak())

# ==================== FINAL PAGE ====================
elements.append(Spacer(1, 1*inch))
elements.append(Paragraph("ADDITIONAL RESOURCES", heading_style))
elements.append(Spacer(1, 0.2*inch))

elements.append(Paragraph("""
<b>To Enhance This Document:</b><br/>
<br/>
1. <b>Add Screenshots:</b><br/>
   • Capture UI at 1280x720 resolution<br/>
   • Annotate with arrows and callouts<br/>
   • Save in screenshots/ folder<br/>
<br/>
2. <b>Add Database Diagrams:</b><br/>
   • Create ER diagram in Lucidchart/Draw.io<br/>
   • Export as PNG/SVG<br/>
   • Include in PDF<br/>
<br/>
3. <b>API Documentation:</b><br/>
   • Document all controller endpoints<br/>
   • Include request/response examples<br/>
   • Add HTTP status codes<br/>
<br/>
4. <b>User Role Permissions:</b><br/>
   • Define Admin vs User rights<br/>
   • Create permission matrix<br/>
   • Document access restrictions<br/>
<br/>
<b>Contact for Support:</b><br/>
• Developer: Your Development Team<br/>
• Created: February 2026<br/>
• Last Updated: {}<br/>
• Version: 1.0 (Enhanced)<br/>
""".format(datetime.now().strftime('%B %d, %Y')), body_style))

elements.append(Spacer(1, 0.5*inch))
elements.append(Paragraph("Thank you for using Contact Management System!", styles['Heading2']))

# Build PDF
doc.build(elements)
print(f"✓ Enhanced Manual PDF created: {pdf_file}")
print(f"✓ Location: e:\\Contact_Management_System\\{pdf_file}")
print(f"\n📋 ADDED SECTIONS:")
print("   ✓ System Architecture")
print("   ✓ Database Schema (8 tables)")
print("   ✓ Detailed Table Specifications")
print("   ✓ Entity Relationships")
print("   ✓ Data Types & Constraints")
print("   ✓ Validation Rules")
print("   ✓ Index Specifications")
print("   ✓ Screenshot Integration Guide")
