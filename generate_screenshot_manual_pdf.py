#!/usr/bin/env python3
"""
Generate a screenshot-based user manual PDF.
"""

from datetime import datetime
from pathlib import Path

from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib.units import inch
from reportlab.lib.enums import TA_CENTER
from reportlab.lib import colors
from reportlab.platypus import SimpleDocTemplate, Paragraph, Spacer, PageBreak, Image
from reportlab.lib.utils import ImageReader


BASE_DIR = Path(__file__).resolve().parent
SCREENSHOT_DIR = BASE_DIR / "screenshots"
OUTPUT_PDF = BASE_DIR / "Contact_Management_System_Screenshot_Manual.pdf"


def scale_image(image_path, max_width):
    """Return an Image flowable scaled to max width while keeping aspect ratio."""
    reader = ImageReader(str(image_path))
    width, height = reader.getSize()
    if width == 0 or height == 0:
        return None
    scale = max_width / float(width)
    return Image(str(image_path), width=width * scale, height=height * scale)


def build_manual():
    styles = getSampleStyleSheet()
    title_style = ParagraphStyle(
        "TitleStyle",
        parent=styles["Title"],
        alignment=TA_CENTER,
        textColor=colors.HexColor("#003D82"),
        fontSize=24,
        spaceAfter=12,
    )
    heading_style = ParagraphStyle(
        "HeadingStyle",
        parent=styles["Heading2"],
        textColor=colors.HexColor("#0EA5E9"),
        spaceBefore=12,
        spaceAfter=8,
    )
    body_style = styles["BodyText"]

    doc = SimpleDocTemplate(
        str(OUTPUT_PDF),
        pagesize=A4,
        topMargin=0.7 * inch,
        bottomMargin=0.7 * inch,
        leftMargin=0.7 * inch,
        rightMargin=0.7 * inch,
    )

    content = []
    content.append(Paragraph("Contact Management System", title_style))
    content.append(Paragraph("Screenshot User Manual", title_style))
    content.append(Spacer(1, 0.2 * inch))
    content.append(
        Paragraph(
            f"Generated on {datetime.now().strftime('%Y-%m-%d')}",
            ParagraphStyle("DateStyle", parent=body_style, alignment=TA_CENTER),
        )
    )
    content.append(Spacer(1, 0.4 * inch))
    content.append(
        Paragraph(
            "This manual explains each major screen and shows the corresponding screenshot.",
            body_style,
        )
    )
    content.append(PageBreak())

    sections = [
        {
            "title": "1) Login",
            "summary": "Secure access to the system.",
            "steps": [
                "Enter your username.",
                "Enter your password.",
                "Click Login to continue.",
            ],
            "images": ["Login.png"],
        },
        {
            "title": "2) Dashboard",
            "summary": "Overview of totals, shortcuts, and recent activity.",
            "steps": [
                "Review summary cards for quick status.",
                "Use shortcut buttons for common actions.",
                "Scroll to review recent contacts or updates.",
            ],
            "images": ["Dashboard.png"],
        },
        {
            "title": "3) All Contacts - Main List",
            "summary": "View all contacts and access common actions.",
            "steps": [
                "Use search to find contacts by name, phone, or email.",
                "Use action buttons to view, edit, or delete.",
                "Use pagination to move through pages.",
            ],
            "images": ["All_Contacts_View.png", "All_Contacts_View_1.png"],
        },
        {
            "title": "4) Add New Contact",
            "summary": "Create a new contact record.",
            "steps": [
                "Enter First Name and/or Last Name (required).",
                "Fill email and phone numbers as needed.",
                "Add address and notes for more context.",
                "Click Create Contact to save.",
            ],
            "images": [
                "Add_New_Contact.png",
                "Add_New_Contact_1.png",
                "Add_New_Contact_2.png",
                "Add_New_Contact_3.png",
            ],
        },
        {
            "title": "5) Edit Contact Details",
            "summary": "Update an existing contact.",
            "steps": [
                "Modify fields that changed.",
                "Review contact numbers and email for accuracy.",
                "Update address and optional fields if needed.",
                "Click Update Contact to save changes.",
            ],
            "images": [
                "Edit_Contact_Details.png",
                "Edit_Contact_Details_1.png",
                "Edit_Contact_Details_2.png",
                "Edit_Contact_Details_3.png",
            ],
        },
        {
            "title": "6) View Contact Details",
            "summary": "Review all information for a single contact.",
            "steps": [
                "Review contact info at the top.",
                "Scroll to see full address and details.",
                "Use action buttons to edit, delete, or manage items.",
            ],
            "images": [
                "View_Contact_Details.png",
                "View_Contact_Details_1.png",
                "View_Contact_Details_2.png",
            ],
        },
        {
            "title": "7) Import Contacts",
            "summary": "Import contacts in bulk from Excel or CSV.",
            "steps": [
                "Download a template (Excel or CSV).",
                "Choose file type and select your file.",
                "Click Import Contacts and review the result.",
            ],
            "images": [
                "Import_Contacts.png",
                "Import_Contacts_1.png",
                "Import_Contacts_2.png",
            ],
        },
    ]

    max_width = doc.width

    for section in sections:
        content.append(Paragraph(section["title"], heading_style))
        content.append(Paragraph(section["summary"], body_style))
        content.append(Spacer(1, 0.1 * inch))
        content.append(Paragraph("Steps:", body_style))
        for idx, step in enumerate(section["steps"], start=1):
            content.append(Paragraph(f"{idx}. {step}", body_style))
        content.append(Spacer(1, 0.1 * inch))

        for image_name in section["images"]:
            image_path = SCREENSHOT_DIR / image_name
            if image_path.exists():
                image_flowable = scale_image(image_path, max_width)
                if image_flowable:
                    content.append(image_flowable)
                    content.append(Spacer(1, 0.2 * inch))
            else:
                content.append(
                    Paragraph(f"[Missing image: {image_name}]", body_style)
                )

        content.append(PageBreak())

    doc.build(content)


if __name__ == "__main__":
    build_manual()
    print(f"PDF generated: {OUTPUT_PDF}")
