# 📘 Contact Management System - Complete User Manual

## Table of Contents
1. [Introduction](#introduction)
2. [Getting Started](#getting-started)
3. [Login Screen](#login-screen)
4. [Dashboard](#dashboard)
5. [Contact Management](#contact-management)
6. [Photo Management](#photo-management)
7. [Document Management](#document-management)
8. [Import/Export Features](#importexport-features)
9. [Administration](#administration)
10. [Tips & Best Practices](#tips--best-practices)

---

## Introduction

The Contact Management System is a comprehensive application designed to help you organize, manage, and maintain your contacts efficiently. This user-friendly system allows you to store contact information, upload photos, attach documents, and manage user access with ease.

### Key Features
- ✅ Add, edit, and delete contacts
- ✅ Upload and manage contact photos
- ✅ Attach documents to contacts
- ✅ Import contacts from Excel/CSV files
- ✅ Export contacts to Excel/CSV files
- ✅ Search and filter contacts
- ✅ Find duplicate contacts
- ✅ User and group management
- ✅ Role-based access control
- ✅ Mobile-friendly interface

---

## Getting Started

### System Requirements
- Windows 10 (version 1909+) or Windows 11
- 2 GB RAM minimum (4 GB recommended)
- 500 MB free disk space
- Modern web browser (Chrome, Edge, Firefox, or Safari)

### First Launch
1. Double-click **Run.bat** in the application folder
2. Wait 10-15 seconds for the application to start
3. Your browser will automatically open to http://localhost:5000
4. You'll see the Login screen

---

## Login Screen

### Purpose
The Login screen is your entry point to the Contact Management System. It ensures secure access to your contact database.

### Screen Elements

#### Login Form
- **Username Field**: Enter your username (e.g., admin)
- **Password Field**: Enter your password securely
- **Remember Me**: Check this to stay logged in (optional)
- **Login Button**: Click to access the system

### How to Use

**First-Time Login:**
```
Username: admin
Password: Admin@123
```

**Steps:**
1. Enter your username in the "Username" field
2. Enter your password in the "Password" field
3. (Optional) Check "Remember Me" to stay logged in
4. Click the **Login** button
5. You'll be redirected to the Dashboard

**Important Security Note:**
⚠️ **Change the default password immediately after first login!**

### Troubleshooting
- **Forgot Password**: Contact your system administrator
- **Account Locked**: Contact your system administrator
- **Cannot Login**: Verify username and password are correct

---

## Dashboard

### Purpose
The Dashboard provides a quick overview of your contact database with key statistics and easy access to common actions.

### Screen Elements

#### Statistics Cards
1. **Total Contacts**
   - Shows the total number of contacts in the system
   - Color-coded with icon
   - Click to view all contacts

2. **Active Users**
   - Displays number of registered users
   - Admin only feature
   - Click to manage users

3. **Recent Activity**
   - Shows recently added/modified contacts
   - Quick access to recent changes

4. **Storage Information**
   - Displays database size
   - Shows photos and documents count

#### Quick Actions Section
- **Add New Contact**: Create a new contact quickly
- **Import Contacts**: Upload Excel/CSV files
- **Export All Contacts**: Download your contacts
- **Find Duplicates**: Identify duplicate entries

#### Recent Contacts
- Lists the 10 most recently added or modified contacts
- Quick view with name and contact details
- Click on any contact to view full details

### How to Use

**View Statistics:**
- Simply look at the dashboard cards for instant insights
- Numbers update automatically when data changes

**Quick Actions:**
1. Click any Quick Action button to perform that task
2. You'll be taken directly to the relevant screen

**Access Recent Contacts:**
1. Scroll to the Recent Contacts section
2. Click on any contact name to view full details
3. Use the "View All Contacts" button to see complete list

---

## Contact Management

The Contact Management section is the core of the system, where you work with individual contacts.

---

### 5.1 All Contacts Screen (Index)

### Purpose
The main screen for viewing, searching, and managing all your contacts in one place.

### Screen Elements

#### Top Toolbar
- **Search Bar**: Search contacts by name, email, phone, or city
- **Add Contact Button**: Create a new contact
- **Import Button**: Import contacts from file
- **Export Button**: Export filtered contacts
- **Find Duplicates Button**: Locate duplicate contacts

#### Contacts Table
Displays all contacts with the following columns:
- **Photo**: Contact profile picture (thumbnail)
- **Name**: First and last name
- **Email**: Email address (clickable to send email)
- **Mobile 1**: Primary phone number (clickable to call)
- **Mobile 2**: Secondary phone number
- **City**: Contact's city
- **Actions**: Quick action buttons

#### Action Buttons
For each contact:
- **👁️ View**: See full contact details
- **✏️ Edit**: Modify contact information
- **🗑️ Delete**: Remove contact from system
- **📸 Photos**: Manage contact photos
- **📄 Documents**: Manage attached documents

#### Pagination
- Shows 10-50 contacts per page
- Navigate using Previous/Next buttons
- Jump to specific page numbers

### How to Use

**Search for Contacts:**
1. Click in the search bar at the top
2. Type any part of the contact's information:
   - Name (first or last)
   - Email address
   - Phone number
   - City name
3. Results filter automatically as you type
4. Clear the search box to show all contacts again

**View Contact Details:**
1. Locate the contact in the table
2. Click the **View (👁️)** button
3. You'll see the full Details screen

**Add a New Contact:**
1. Click the **+ Add Contact** button (top right)
2. Fill in the contact form (see Create Contact section)
3. Click **Save**

**Export Contacts:**
1. (Optional) Use search to filter contacts
2. Click the **Export** button
3. Choose Excel or CSV format
4. File downloads automatically

**Change Items Per Page:**
1. Look for the dropdown at the bottom
2. Select 10, 25, or 50 items per page
3. Table updates automatically

### Tips
- 💡 Use search to quickly find specific contacts
- 💡 Click column headers to sort (if enabled)
- 💡 The table is mobile-responsive - works on phones and tablets

---

### 5.2 Create Contact Screen

### Purpose
Add new contacts to your database with all relevant information.

### Screen Elements

#### Personal Information Section
- **First Name** (Required): Contact's first name
- **Last Name** (Required): Contact's last name
- **Nickname** (Optional): Informal name or alias

#### Contact Information Section
- **Email** (Optional): Email address
- **Mobile 1** (Optional): Primary phone number
- **Mobile 2** (Optional): Secondary phone number
- **Mobile 3** (Optional): Third phone number
- **WhatsApp Number** (Optional): WhatsApp contact number

#### Address Information Section
- **Address** (Optional): Street address
- **City** (Optional): City name
- **State** (Optional): State or province
- **Postal Code** (Optional): ZIP or postal code
- **Country** (Optional): Country name

#### Additional Information
- **Other Details** (Optional): Any additional notes (large text area)

#### Action Buttons
- **Create Contact**: Save the new contact
- **Cancel**: Return to contacts list without saving

### How to Use

**Create a New Contact:**
1. Navigate to this screen by clicking **+ Add Contact**
2. Fill in the required fields (marked with *)
   - At least First Name OR Last Name is required
3. Fill in optional fields as desired
4. Enter any additional notes in "Other Details"
5. Click **Create Contact** button
6. You'll see a success message
7. You'll be redirected to the All Contacts screen

**Tips for Data Entry:**
- Enter phone numbers in any format: +1234567890, (123) 456-7890, etc.
- Use the Tab key to move between fields quickly
- Email addresses are validated automatically
- You can leave optional fields blank

**Required Fields:**
⚠️ At least one of the following is required:
- First Name
- Last Name

### Example
```
First Name: John
Last Name: Doe
Email: john.doe@company.com
Mobile 1: +1 (555) 123-4567
City: New York
State: NY
Country: USA
Other Details: Met at conference 2026
```

---

### 5.3 Edit Contact Screen

### Purpose
Modify existing contact information to keep your database up-to-date.

### Screen Elements

The Edit screen has the same layout as the Create screen, but:
- All fields are pre-filled with existing data
- Page title shows "Edit Contact - [Name]"
- Button says "Update Contact" instead of "Create Contact"

### How to Use

**Edit a Contact:**
1. From All Contacts screen, click **Edit (✏️)** button
2. The Edit screen opens with current information
3. Modify any fields you wish to change
4. Click **Update Contact** to save changes
5. Or click **Cancel** to discard changes
6. You'll see a success message
7. You'll be redirected to Contact Details or All Contacts

**Common Editing Tasks:**
- Update phone number when it changes
- Add email address if it was missing
- Correct spelling mistakes
- Add new address information
- Update notes and additional details

### Tips
- 💡 You can clear optional fields by deleting the text
- 💡 Changes are not saved until you click "Update Contact"
- 💡 Use Cancel if you change your mind

---

### 5.4 Contact Details Screen

### Purpose
View complete information about a specific contact in a clean, organized layout.

### Screen Elements

#### Contact Header
- Large profile photo (or default avatar if no photo)
- Contact's full name (prominent display)
- Quick action buttons in top right

#### Quick Action Buttons
- **✏️ Edit**: Modify contact details
- **🗑️ Delete**: Remove this contact
- **📸 Manage Photos**: Go to photo gallery
- **📄 Manage Documents**: View attached documents
- **⬅️ Back to Contacts**: Return to list

#### Information Sections

**Contact Information Card:**
- Email address (clickable to send email)
- Mobile 1 (clickable to call/SMS)
- Mobile 2
- Mobile 3
- WhatsApp Number (with WhatsApp link)

**Address Information Card:**
- Full street address
- City, State, Postal Code
- Country

**Additional Details Card:**
- Other notes and information
- Free-form text area

#### Photos Section
- Displays all uploaded photos (thumbnails)
- "View All Photos" button
- "Add Photo" button

#### Documents Section
- Lists all attached documents with icons
- Document names and file types
- Download buttons for each document
- "Manage Documents" button

### How to Use

**View Contact Details:**
1. Click on any contact from the All Contacts list
2. The Details screen opens showing all information
3. Scroll down to see photos and documents

**Send Email to Contact:**
1. Find the email address in Contact Information
2. Click on the email address
3. Your default email program opens with a new message

**Call or SMS Contact:**
1. Find the phone number in Contact Information
2. Click on the phone number
3. Your system will open the appropriate application

**Contact via WhatsApp:**
1. Find the WhatsApp Number
2. Click the green WhatsApp button
3. WhatsApp Web opens with a chat

**Quick Navigation:**
- Click **Edit** to modify information
- Click **Manage Photos** to add/view photos
- Click **Manage Documents** to add/view files
- Click **Back to Contacts** to return to the list

---

### 5.5 Delete Contact Screen

### Purpose
Safely remove a contact from the system with confirmation to prevent accidental deletion.

### Screen Elements

#### Warning Section
- ⚠️ Red warning box with clear message
- Contact's name and key information displayed
- Explanation of consequences

#### Confirmation Form
- **Confirmation checkbox**: "Yes, I want to delete this contact"
- **Delete button**: Permanently remove (red color)
- **Cancel button**: Return without deleting

### How to Use

**Delete a Contact:**
1. From Details or All Contacts, click **Delete (🗑️)** button
2. Read the warning message carefully
3. Review the contact information shown
4. Check the confirmation checkbox
5. Click the red **Delete** button
6. Contact is permanently removed
7. You'll see a success message

**Cancel Deletion:**
1. Simply click the **Cancel** button
2. Or click your browser's Back button
3. Contact remains unchanged

### Important Notes
⚠️ **Warning:** Deletion is permanent and cannot be undone!
- All contact photos will be deleted
- All attached documents will be deleted
- All contact information will be removed from the database

**Before Deleting:**
- Make sure you have the right contact
- Export the contact if you might need the data later
- Check if you should merge with another contact instead

---

### 5.6 Find Duplicates Screen

### Purpose
Identify potential duplicate contacts in your database to maintain data quality.

### Screen Elements

#### Scan Options
- **Scan Method**: Choose how to find duplicates
  - By Name (First Name + Last Name)
  - By Email Address
  - By Phone Number
- **Start Scan Button**: Begin the search

#### Results Section
- **Duplicate Groups**: Contacts that might be duplicates
- **Contact Cards**: Show key information for each potential duplicate
- **Action Buttons**: 
  - View Details
  - Keep This One
  - Delete

#### Statistics
- Number of duplicate groups found
- Total contacts involved

### How to Use

**Find Duplicate Contacts:**
1. Click **Find Duplicates** from Dashboard or menu
2. Select your preferred scan method:
   - **By Name**: Finds contacts with exact same first and last names
   - **By Email**: Finds contacts with same email address
   - **By Phone**: Finds contacts with same phone number
3. Click **Start Scan**
4. Wait for results (usually takes a few seconds)
5. Review the duplicate groups shown

**Handle Duplicates:**

**Method 1: Keep One, Delete Others**
1. Review each duplicate group
2. Click **View Details** to see full information
3. Decide which contact to keep (usually the most complete one)
4. Click **Delete** on the ones you want to remove
5. Confirm deletion

**Method 2: Merge Information**
1. View details of both/all duplicates
2. Choose the best one to keep
3. Click **Edit** on that contact
4. Manually add any missing information from other duplicates
5. Return and delete the duplicate entries

### Tips
- 💡 Run duplicate scans regularly (monthly)
- 💡 Scan by name first, then by email, then by phone
- 💡 Always review before deleting - some similar contacts might be different people (e.g., family members)
- 💡 Export contacts before bulk duplicate deletion

### Common Scenarios
- **Same person entered twice**: Keep the most complete record
- **Old and new phone numbers**: Update one record, delete other
- **Spelling variations**: Keep correct spelling, delete typo
- **Family members**: These are NOT duplicates - keep separate

---

## Photo Management

### 6.1 Photo Gallery Screen

### Purpose
View, upload, and manage photos for a specific contact. Store multiple photos including profile pictures, ID photos, or any relevant images.

### Screen Elements

#### Contact Header
- Contact's name and basic info
- Current profile photo indicator
- Back to Contact Details button

#### Upload Section
- **Choose File Button**: Select photo from your computer
- **Upload Button**: Send photo to server
- Supported formats shown (JPG, PNG, etc.)
- Maximum file size displayed

#### Photo Gallery
- Grid layout of all photos
- Thumbnail previews
- Photo filenames or descriptions
- Action buttons for each photo:
  - **🔍 View Full Size**: Open large version
  - **⭐ Set as Profile**: Make this the main photo
  - **🗑️ Delete**: Remove photo

#### Profile Photo Indicator
- Shows which photo is the current profile picture
- Highlighted with star icon or border

### How to Use

**Upload a New Photo:**
1. Navigate to contact's Details screen
2. Click **Manage Photos** or **📸** button
3. Click **Choose File** button
4. Browse your computer and select a photo
5. Supported formats: JPG, JPEG, PNG, GIF
6. Maximum size: Usually 5 MB
7. Click **Upload** button
8. Photo appears in gallery after upload
9. You'll see a success message

**Set Profile Photo:**
1. View all photos in the gallery
2. Find the photo you want as main profile picture
3. Click **Set as Profile** (⭐) button
4. Photo becomes the main contact image
5. This photo shows in:
   - Contact list thumbnails
   - Contact details header
   - Search results

**View Full-Size Photo:**
1. Click **View Full Size** (🔍) on any photo
2. Photo opens in larger view
3. Can zoom in for details
4. Close to return to gallery

**Delete a Photo:**
1. Click **Delete** (🗑️) button on unwanted photo
2. Confirm the deletion
3. Photo is permanently removed

**Tips:**
- 💡 Upload clear, well-lit photos
- 💡 Crop photos before uploading for best results
- 💡 Name files descriptively (e.g., "John_Doe_ID_2026.jpg")
- 💡 Always set a profile photo - it helps identify contacts quickly
- 💡 You can store multiple photos per contact (ID, passport, business card, etc.)

### Best Practices
- Use consistent photo sizes
- Ensure photos are properly oriented
- Remove blurry or poor-quality photos
- Keep photo library organized
- Update photos when they become outdated

---

## Document Management

### 7.1 Document List Screen

### Purpose
Attach, view, and manage documents related to a specific contact, such as contracts, resumes, certificates, or any relevant files.

### Screen Elements

#### Contact Header
- Contact's name and basic info
- Back to Contact Details button

#### Upload Section
- **Choose File Button**: Select document from computer
- **Document Description Field**: Add optional description
- **Upload Button**: Send document to server
- Supported file types displayed
- Maximum file size shown

#### Documents Table
Displays all attached documents with columns:
- **📄 Icon**: File type icon (PDF, Word, Excel, etc.)
- **Filename**: Original document name
- **Description**: Optional description text
- **Upload Date**: When document was added
- **File Size**: Document size
- **Actions**: Action buttons

#### Action Buttons per Document
- **📥 Download**: Save document to your computer
- **👁️ View**: Preview document in browser (if supported)
- **✏️ Edit Description**: Change document description
- **🗑️ Delete**: Remove document

### How to Use

**Upload a Document:**
1. Navigate to contact's Details screen
2. Click **Manage Documents** or **📄** button
3. Click **Choose File** button
4. Browse your computer and select a document
5. Supported formats:
   - PDF (.pdf)
   - Word (.doc, .docx)
   - Excel (.xls, .xlsx)
   - Images (.jpg, .png)
   - Text (.txt)
   - And more
6. (Optional) Enter a description in the description field
   - Example: "Employment Contract 2026"
   - Example: "Resume - Updated Feb 2026"
7. Click **Upload** button
8. Document appears in the list
9. You'll see a success message

**Download a Document:**
1. Find the document in the list
2. Click the **Download** (📥) button
3. Document downloads to your computer
4. Opens in default application for that file type

**View Document:**
1. Click the **View** (👁️) button
2. Document opens in browser (if supported format)
3. PDF files show in browser viewer
4. Close tab when done viewing

**Add or Edit Description:**
1. When uploading, fill in the description field
2. For existing documents, click **Edit Description**
3. Enter meaningful description
4. Click **Save**
5. Description helps you identify documents later

**Delete a Document:**
1. Click **Delete** (🗑️) button
2. Confirm deletion in popup
3. Document is permanently removed

### Use Cases

**Employment Documents:**
- Resume/CV
- Job application
- Employment contract
- Performance reviews
- Certifications

**Personal Documents:**
- ID copies
- Passport copies
- Certificates
- Licenses

**Business Documents:**
- Contracts
- Agreements
- Meeting notes
- Proposals
- Invoices

### Tips
- 💡 Always add descriptions - makes finding documents easier
- 💡 Use consistent naming convention for filenames
- 💡 Organize documents by type or date
- 💡 Scan paper documents and attach digital copies
- 💡 Keep document file sizes reasonable (under 10 MB)
- 💡 Remove outdated documents regularly

### Security Notes
- 📁 Documents are stored securely on the server
- 🔒 Only authorized users can access documents
- 🗝️ Each contact's documents are private
- 💾 Regular backups protect your documents

---

## Import/Export Features

### 8.1 Import Contacts Screen

### Purpose
Bulk import contacts from Excel or CSV files, saving time when migrating data or adding multiple contacts at once.

### Screen Elements

#### Instructions Section
- Step-by-step guide
- Requirements listed
- Helpful tips displayed

#### Download Template Section
- **Download Excel Template** button (green)
- **Download CSV Template** button (blue)
- Sample template included with proper format

#### Import Form
- **File Type Selector**: Choose Excel or CSV
- **File Browser**: Select file from computer
- **Import Button**: Start the import process
- **Cancel Button**: Return to contacts

#### File Format Information Table
Shows all supported columns:
- Column number
- Column name
- Required/Optional status
- Example data

### How to Use

**Import Contacts (Complete Process):**

**Step 1: Download Template**
1. Click **Download Excel Template** or **Download CSV Template**
2. Template file downloads to your computer
3. Open the template in Excel or text editor

**Step 2: Prepare Your Data**
1. Open the downloaded template
2. Fill in your contact information
3. Follow the column order shown in the table:
   - Column A: FirstName (Required*)
   - Column B: LastName (Required*)
   - Column C: NickName (Optional)
   - Column D: Email (Optional)
   - Column E: Mobile1 (Optional)
   - Column F: Mobile2 (Optional)
   - Column G: Mobile3 (Optional)
   - Column H: WhatsAppNumber (Optional)
   - Column I: Address (Optional)
   - Column J: City (Optional)
   - Column K: State (Optional)
   - Column L: PostalCode (Optional)
   - Column M: Country (Optional)
   - Column N: OtherDetails (Optional)

4. **Important:** At least FirstName OR LastName is required
5. Remove the sample row before importing
6. Save the file

**Step 3: Import the File**
1. Go to Import Contacts screen
2. Select the file type (Excel or CSV) from dropdown
3. Click **Choose File** and select your prepared file
4. Verify the file name appears
5. Click **Import Contacts** button
6. Wait for processing (may take a few seconds for large files)

**Step 4: Review Results**
1. You'll see a success message showing:
   - Number of contacts imported successfully
   - Any errors or skipped rows
2. Click **Back to Contacts** to view imported contacts

### Template Format Example

**Excel/CSV Structure:**
```
FirstName | LastName | Email | Mobile1 | City | State
John | Doe | john@email.com | +1234567890 | New York | NY
Jane | Smith | jane@email.com | +0987654321 | Los Angeles | CA
```

### Import Rules

**Data Validation:**
- ✅ At least FirstName OR LastName required per row
- ✅ Email addresses are validated for proper format
- ✅ Duplicate contacts are handled based on settings
- ✅ Empty rows are skipped automatically
- ✅ Phone numbers accept various formats

**What Happens:**
- New contacts are created for each valid row
- Invalid rows are skipped with error message
- Existing contacts are NOT updated (imports add new only)
- You'll get a summary report after import

### Tips for Successful Import
- 💡 Always start with the provided template
- 💡 Keep column order exact as shown
- 💡 Don't change column headers
- 💡 Remove extra rows/columns not in template
- 💡 Test with small file first (5-10 contacts)
- 💡 Check for duplicates before importing
- 💡 Save Excel as .xlsx (not .xls)
- 💡 Use UTF-8 encoding for CSV files

### Common Errors and Solutions

**Error: File format not recognized**
- Solution: Make sure you selected correct file type (Excel/CSV)
- Solution: Verify file is not corrupted

**Error: Required fields missing**
- Solution: Ensure each row has FirstName OR LastName
- Solution: Check column order matches template

**Error: Invalid email format**
- Solution: Fix email addresses to proper format (user@domain.com)

**Error: File too large**
- Solution: Split into smaller files (500 contacts per file max)

### Best Practices
1. **Backup First**: Export existing contacts before importing
2. **Clean Data**: Remove duplicates in your source file
3. **Standardize Format**: Use consistent phone/address formats
4. **Test Small**: Import 5 contacts first to verify format
5. **Review After**: Check imported contacts for accuracy

---

### 8.2 Export Contacts Screen

### Purpose
Download your contacts to Excel or CSV format for backup, reporting, or use in other systems.

### How to Use

**Export All Contacts:**
1. From Dashboard or All Contacts screen
2. Click **Export** button
3. Choose format:
   - **Excel (.xlsx)**: Formatted spreadsheet with nice layout
   - **CSV (.csv)**: Plain text, compatible with all systems
4. File downloads automatically to your Downloads folder
5. Open file to view exported contacts

**Export Filtered Contacts:**
1. Go to All Contacts screen
2. Use search to filter contacts (by city, name, etc.)
3. Click **Export** button
4. Only filtered contacts are exported
5. Useful for creating targeted lists

### Export File Contents

**Includes All Fields:**
- First Name, Last Name, Nickname
- Email Address
- Mobile 1, Mobile 2, Mobile 3
- WhatsApp Number
- Address, City, State, Postal Code, Country
- Other Details

**Excel Format Features:**
- Header row with column names
- Formatted cells
- Auto-sized columns
- Professional appearance

**CSV Format Features:**
- Plain text format
- Comma-separated values
- Universal compatibility
- Smaller file size

### Use Cases

**Backup:**
- Regular backup of contact database
- Before making bulk changes
- Before system maintenance

**Reporting:**
- Contact list for distribution
- Marketing campaigns
- Directory creation

**Data Migration:**
- Moving to another system
- Sharing with colleagues
- Archival purposes

### Tips
- 💡 Export regularly for backups (weekly recommended)
- 💡 Use Excel for better formatting
- 💡 Use CSV for importing to other systems
- 💡 Include date in filename (e.g., "Contacts_Feb_2026.xlsx")
- 💡 Store backups securely

---

## Administration

*Note: Admin features require Administrator role access*

---

### 9.1 Users Management Screen

### Purpose
Manage system users, create new accounts, edit user details, and control access permissions.

### Screen Elements

#### Users Table
Displays all registered users with columns:
- **Username**: Login username
- **Full Name**: User's real name
- **Email**: User's email address
- **Role**: User's role (Admin, Manager, User, etc.)
- **Group**: Assigned user group
- **Status**: Active or Inactive
- **Actions**: Edit and Delete buttons

#### Action Buttons
- **+ Create New User**: Add a new user account
- **Edit**: Modify user details
- **Delete**: Remove user account
- **🔑 User Rights**: Manage specific user permissions

### How to Use

**View All Users:**
1. Click **Administration** menu
2. Select **Users**
3. See complete list of all system users

**Create New User:**
(See section 9.2 - Create User Screen)

**Edit User:**
(See section 9.3 - Edit User Screen)

**Delete User:**
1. Find user in the table
2. Click **Delete** button
3. Confirm deletion
4. User account is removed
5. ⚠️ User's data (contacts added by them) remains in system

**Manage User Rights:**
1. Click **User Rights** button (🔑)
2. See specific permissions screen
3. Grant or revoke individual permissions
4. Save changes

### User Roles Explained

**Administrator:**
- Full system access
- Can manage users and groups
- Can see all contacts
- Can delete any data

**Manager:**
- Can manage contacts
- Can import/export
- Limited admin functions
- Cannot delete users

**User:**
- Can view and edit contacts
- Cannot access admin features
- Basic permissions only

### Tips
- 💡 Don't delete your own admin account
- 💡 Always have at least one active administrator
- 💡 Review user access regularly
- 💡 Deactivate instead of delete when users leave

---

### 9.2 Create User Screen

### Purpose
Add new user accounts to the system with appropriate roles and permissions.

### Screen Elements

#### User Information Form
- **Username** (Required): Login username (unique)
- **Full Name** (Required): User's real name
- **Email** (Required): Email address
- **Password** (Required): Initial password
- **Confirm Password** (Required): Re-enter password
- **Role** (Required): Select role from dropdown
- **Group** (Optional): Assign to user group
- **Status**: Active or Inactive

#### Action Buttons
- **Create User**: Save new user account
- **Cancel**: Return without creating

### How to Use

**Create a New User:**
1. Go to Users screen
2. Click **+ Create New User** button
3. Fill in all required fields:

**Username:**
- Must be unique
- 3-20 characters
- No spaces
- Example: "jdoe" or "john.doe"

**Full Name:**
- User's complete name
- Example: "John Doe"

**Email:**
- Valid email address
- Used for notifications and password reset
- Example: "john.doe@company.com"

**Password:**
- Minimum 6 characters
- Should include letters and numbers
- Example: "JohnDoe@123"

**Confirm Password:**
- Must match password exactly

**Role:**
- Select from dropdown:
  - Administrator
  - Manager
  - User
- Choose appropriate role for user's responsibilities

**Group:**
- (Optional) Select user group
- Groups help organize users by department or function

**Status:**
- Active: User can log in
- Inactive: Account disabled (cannot log in)

4. Click **Create User** button
5. You'll see success message
6. New user can now log in with created credentials
7. ⚠️ Inform user of their username and password securely

### Tips
- 💡 Use clear, memorable usernames
- 💡 Create strong initial passwords
- 💡 Ask users to change password on first login
- 💡 Assign minimum required role/permissions
- 💡 Add email for password recovery

---

### 9.3 Edit User Screen

### Purpose
Modify existing user account information, change roles, reset passwords, or update user status.

### Screen Elements

Same form as Create User screen, but:
- Fields are pre-filled with current information
- Username usually cannot be changed
- Password fields are optional (leave blank to keep current)
- Page title shows "Edit User - [Username]"

### How to Use

**Edit User Information:**
1. Go to Users screen
2. Click **Edit** button for desired user
3. Modify any fields you wish to change:
   - Update full name if changed
   - Change email address
   - Adjust role if responsibilities changed
   - Change group assignment
   - Activate/deactivate account
4. Click **Update User** button
5. Changes are saved

**Reset User Password:**
1. Edit the user
2. Enter new password in Password field
3. Confirm new password
4. Click **Update User**
5. Inform user of new password securely

**Deactivate User:**
1. Edit the user
2. Change Status to "Inactive"
3. Click **Update User**
4. User cannot log in anymore
5. Account and data preserved

**Reactivate User:**
1. Edit the inactive user
2. Change Status to "Active"
3. Click **Update User**
4. User can log in again

### Tips
- 💡 Leave password fields blank to keep current password
- 💡 Deactivate instead of delete to preserve audit trail
- 💡 Document reason for role changes
- 💡 Verify email addresses are current

---

### 9.4 User Rights Screen

### Purpose
Manage granular permissions for individual users, controlling exactly what actions each user can perform.

### Screen Elements

#### User Information
- Username and full name displayed
- Current role shown

#### Permissions Checklist
Categories of permissions with checkboxes:

**Contact Management:**
- ☐ View Contacts
- ☐ Create Contacts
- ☐ Edit Contacts
- ☐ Delete Contacts
- ☐ Export Contacts

**Photo Management:**
- ☐ View Photos
- ☐ Upload Photos
- ☐ Delete Photos

**Document Management:**
- ☐ View Documents
- ☐ Upload Documents
- ☐ Delete Documents

**Import/Export:**
- ☐ Import Contacts
- ☐ Export All Contacts

**Administration:**
- ☐ Manage Users
- ☐ Manage Groups
- ☐ View System Logs

#### Action Buttons
- **Save Permissions**: Apply changes
- **Reset to Default**: Restore role defaults
- **Cancel**: Discard changes

### How to Use

**Set User Permissions:**
1. Go to Users screen
2. Click **User Rights** (🔑) for desired user
3. Review current permissions (checked boxes)
4. Check boxes to grant permissions
5. Uncheck boxes to revoke permissions
6. Click **Save Permissions**
7. User's access updates immediately

**Grant Additional Permissions:**
1. Open User Rights for user
2. Check additional permission boxes needed
3. Click **Save Permissions**

**Restrict User Access:**
1. Open User Rights for user
2. Uncheck permissions to remove
3. Click **Save Permissions**
4. User loses access to those features immediately

**Reset to Role Defaults:**
1. Click **Reset to Default** button
2. Permissions reset to standard for user's role
3. Click **Save Permissions** to apply

### Permission Combinations

**Read-Only User:**
- ✓ View Contacts
- ✗ Create/Edit/Delete

**Data Entry User:**
- ✓ View Contacts
- ✓ Create Contacts
- ✓ Edit Contacts
- ✗ Delete Contacts

**Full Access User:**
- ✓ All Contact permissions
- ✓ All Photo/Document permissions
- ✗ Administration

### Tips
- 💡 Follow principle of least privilege
- 💡 Grant only permissions needed for job role
- 💡 Review permissions quarterly
- 💡 Document special permission grants
- 💡 Test user access after changes

---

### 9.5 Groups Management Screen

### Purpose
Organize users into groups based on departments, teams, or functions. Apply permissions to entire groups efficiently.

### Screen Elements

#### Groups Table
- **Group Name**: Name of the group
- **Description**: Group purpose
- **Members Count**: Number of users in group
- **Actions**: Edit, Delete, Manage Rights buttons

#### Action Buttons
- **+ Create New Group**: Add new user group
- **Edit**: Modify group details
- **Delete**: Remove group
- **🔑 Group Rights**: Manage group permissions

### How to Use

**Create a New Group:**
(See section 9.6 - Create Group Screen)

**Edit Group:**
1. Click **Edit** button for desired group
2. Modify name or description
3. Add or remove members
4. Click **Update Group**

**Delete Group:**
1. Click **Delete** button
2. Confirm deletion
3. ⚠️ Users in group remain, just lose group assignment

**Manage Group Rights:**
1. Click **Group Rights** (🔑) button
2. Set permissions for entire group
3. All group members inherit these permissions
4. Individual user rights can override

### Group Examples

**Management Group:**
- Managers and supervisors
- Full access to contacts
- Can export and import
- Cannot manage users

**Sales Team:**
- Sales representatives
- Can view and edit contacts
- Can upload documents
- Can export their own contacts

**View Only Group:**
- Temporary staff or consultants
- Can only view contacts
- No edit or delete permissions
- No export capabilities

### Tips
- 💡 Create groups by function, not hierarchy
- 💡 Use descriptive group names
- 💡 Set group permissions first, then individual exceptions
- 💡 Review group memberships regularly
- 💡 Document group purposes

---

### 9.6 Create Group Screen

### Purpose
Create new user groups to organize users and manage permissions efficiently.

### Screen Elements

#### Group Information Form
- **Group Name** (Required): Unique group name
- **Description** (Optional): Purpose of group
- **Members**: Select users to add to group

#### Action Buttons
- **Create Group**: Save new group
- **Cancel**: Return without creating

### How to Use

**Create a Group:**
1. Go to Groups screen
2. Click **+ Create New Group**
3. Enter Group Name (e.g., "Sales Team", "Managers", "IT Support")
4. Enter Description (e.g., "Sales department staff members")
5. (Optional) Select initial members from user list
   - Hold Ctrl to select multiple
6. Click **Create Group**
7. Group is created and ready for use
8. Set group permissions using Group Rights

---

### 9.7 Edit Group Screen

### Purpose
Modify group information and membership.

### How to Use

**Edit a Group:**
1. Click **Edit** on desired group
2. Change name or description if needed
3. Add new members by selecting from list
4. Remove members by deselecting
5. Click **Update Group**

---

### 9.8 Group Rights Screen

### Purpose
Set permissions for an entire group. All members inherit these permissions.

### Screen Elements

Same as User Rights screen, but applies to all group members.

### How to Use

**Set Group Permissions:**
1. Click **Group Rights** (🔑) for desired group
2. Check/uncheck permissions for entire group
3. Click **Save Permissions**
4. All group members inherit these permissions
5. Individual user rights can override group rights

### Permission Hierarchy

**Priority Order (highest to lowest):**
1. Individual User Rights
2. Group Rights
3. Role Defaults

**Example:**
- Group: Can view contacts
- User override: Can also edit contacts
- Result: That user can view AND edit

### Tips
- 💡 Set common permissions at group level
- 💡 Use individual user rights for exceptions
- 💡 Test with a new user to verify permissions work
- 💡 Document why groups have specific permissions

---

### 9.9 Access Denied Screen

### Purpose
Informs users when they attempt to access features they don't have permission for.

### Screen Elements  
- Clear message explaining access is denied
- Reason for denial (if available)
- Link to return to previous page
- Contact administrator message

### What to Do

**If You See This Screen:**
1. You don't have permission for that action
2. Click **Go Back** to return
3. Contact your administrator if you need access
4. Provide details about what you were trying to do

**For Administrators:**
- If user should have access, check their user rights
- If it's a mistake, grant appropriate permission
- If intentional, explain to user why they don't have access

---

## Tips & Best Practices

### Data Management Best Practices

**1. Regular Maintenance**
- Run duplicate scans monthly
- Remove outdated contacts quarterly
- Verify contact information accuracy

**2. Data Entry Standards**
- Use consistent formatting for phone numbers
- Enter complete addresses when possible
- Add meaningful notes in Other Details
- Always set profile photos

**3. Backup Strategy**
- Export contacts weekly
- Save exports with date in filename
- Keep exports in secure location
- Test restore process periodically

**4. Importing Data**
- Clean source data before importing
- Remove duplicates in source file
- Test import with small file first
- Review imported data immediately

**5. Search Effectively**
- Use partial names to cast wider net
- Search by city for location-based lists
- Use email domain for company contacts
- Clear search to see all results

### Security Best Practices

**1. Password Management**
- Change default password immediately
- Use strong passwords (8+ characters, mixed case, numbers)
- Never share passwords
- Change passwords every 90 days

**2. User Access**
- Grant minimum necessary permissions
- Review user accounts quarterly
- Deactivate accounts when users leave
- Don't create generic shared accounts

**3. Data Protection**
- Log out when leaving computer
- Don't access on public/untrusted computers
- Keep application updated
- Report suspicious activity

### Performance Tips

**1. Keep Database Lean**
- Delete unused contacts
- Remove old documents
- Compress/resize photos before upload
- Clean out duplicate entries

**2. Efficient Searching**
- Use specific search terms
- Clear search between lookups
- Use filters to narrow results
- Limit export sizes

**3. Photo Management**
- Resize large images before upload
- Use JPG for photos (smaller size)
- Delete duplicate/poor photos
- Limit to 3-5 photos per contact

### Workflow Optimization

**1. Daily Tasks**
- Add new contacts as received
- Update changed information immediately
- Review and respond to new contacts
- Quick duplicate check on new entries

**2. Weekly Tasks**
- Export backup of all contacts
- Clean up incomplete records
- Process new documents received
- Update profile photos as needed

**3. Monthly Tasks**
- Run full duplicate scan
- Review inactive contacts
- Clean up old documents
- Audit user permissions (admins)

**4. Quarterly Tasks**
- Major data cleanup
- Verify all contact information
- Archive old documents
- Update procedures

### Keyboard Shortcuts

- **Ctrl + S**: Save (on forms)
- **Esc**: Cancel/Close
- **Tab**: Move to next field
- **Shift + Tab**: Move to previous field
- **Enter**: Submit form (when focused on button)
- **Backspace**: Go back (in browser)

### Mobile Usage Tips

**Accessing on Mobile:**
- Works on phones and tablets
- Navigate to http://localhost:5000 (if on same device)
- For network access, use computer's IP address
- Interface auto-adapts to mobile screen

**Mobile-Optimized Features:**
- Touch-friendly buttons
- Swipe gestures supported
- Responsive tables
- Optimized image sizes

---

## Troubleshooting Common Issues

### Login Issues

**Problem: Cannot log in**
- **Solution 1**: Verify username and password are correct
- **Solution 2**: Check Caps Lock is off
- **Solution 3**: Contact administrator for password reset

**Problem: Forgot password**
- **Solution**: Contact system administrator to reset your password

### Contact Management Issues

**Problem: Can't create contact**
- **Solution**: Ensure at least First Name OR Last Name is filled
- **Check**: Verify you have "Create Contacts" permission
- **Clear browser cache** and try again

**Problem: Photos won't upload**
- **Solution**: Check file size (must be under 5 MB)
- **Solution**: Verify file format (JPG, PNG, GIF only)
- **Solution**: Ensure filename has no special characters

**Problem: Documents won't upload**
- **Solution**: Check file size limit
- **Solution**: Verify file format is supported
- **Solution**: Close file if it's open in another program

### Import Issues

**Problem: Import fails**
- **Solution**: Verify file matches template exactly
- **Solution**: Check column order is correct
- **Solution**: Ensure at least FirstName or LastName in each row
- **Solution**: Save Excel as .xlsx (not .xls)

**Problem: Some rows skipped**
- **Solution**: Check skipped rows for missing required fields
- **Solution**: Verify email format is correct
- **Solution**: Remove completely empty rows

### Performance Issues

**Problem: Application runs slowly**
- **Solution**: Clear browser cache
- **Solution**: Reduce contacts per page (10 instead of 50)
- **Solution**: Close unused browser tabs
- **Solution**: Restart application

**Problem: Search is slow**
- **Solution**: Be more specific in search terms
- **Solution**: Run duplicate cleanup
- **Solution**: Reduce database size by deleting unused contacts

### Browser-Specific Issues

**Problem: Features don't work**
- **Solution**: Update browser to latest version
- **Solution**: Enable JavaScript
- **Solution**: Disable browser extensions temporarily
- **Solution**: Try different browser

**Recommended Browsers:**
- ✅ Google Chrome (latest)
- ✅ Microsoft Edge (latest)
- ✅ Mozilla Firefox (latest)
- ✅ Safari (latest on Mac)

---

## Frequently Asked Questions (FAQ)

### General Questions

**Q: Is internet required?**  
A: No, the application runs completely offline on your computer.

**Q: Where is my data stored?**  
A: All data is stored locally on your computer in a database file in the application folder.

**Q: Can multiple people use it simultaneously?**  
A: If on the same computer, no. For network access, setup is required (contact IT support).

**Q: How many contacts can I store?**  
A: Technically unlimited, but performance is best with under 10,000 contacts.

**Q: Can I access from my phone?**  
A: Yes, if phone is on same network. Use computer's IP address instead of localhost.

### Data Questions

**Q: How do I backup my data?**  
A: Export all contacts to Excel regularly. Keep the database file (.mdf) backed up.

**Q: Can I transfer data to another computer?**  
A: Yes, copy the database file or export/import contacts via Excel/CSV.

**Q: What happens if I delete a contact by mistake?**  
A: Deletion is permanent. Restore from your latest backup export.

**Q: Can I recover deleted photos?**  
A: No, unless you have a backup. Always confirm before deleting.

### Import/Export Questions

**Q: Can I update existing contacts via import?**  
A: No, import only adds new contacts. Manually edit existing ones.

**Q: What's better, Excel or CSV?**  
A: Excel for backup (preserves formatting). CSV for importing to other systems.

**Q: Can I import contacts from Gmail/Outlook?**  
A: Export from Gmail/Outlook to CSV first, then import to this system.

### Administration Questions

**Q: How do I reset a user's password?**  
A: Edit the user account and enter a new password.

**Q: Can users change their own passwords?**  
A: Depends on system setup. Check with administrator.

**Q: What happens when I delete a user?**  
A: User account is removed, but contacts they created remain in the database.

**Q: How many admins should we have?**  
A: At least 2, to ensure admin access if one account has issues.

---

## Contact & Support

### Getting Help

**For Technical Issues:**
1. Check this manual first (search for keywords)
2. Review Troubleshooting section
3. Export your data as backup before making changes
4. Contact your system administrator

**For Feature Requests:**
- Document your request with examples
- Explain how it would help your workflow
- Submit to development team or administrator

**For Security Concerns:**
- Report immediately to system administrator
- Change your password if suspicious activity
- Document what you observed

### System Information

**Application Name**: Contact Management System  
**Version**: 1.0.0  
**Platform**: Windows 10/11  
**Database**: SQL Server LocalDB  
**Browser**: Chrome, Edge, Firefox, Safari supported  

### Additional Resources

- **Quick Start Guide**: START_HERE.md
- **Deployment Guide**: DEPLOYMENT_GUIDE.md
- **Administrator Guide**: Contact system administrator
- **API Documentation**: API_DOCUMENTATION.md

---

## Appendix

### Glossary of Terms

**Administrator**: User with full system access and user management capabilities

**Contact**: A person's record in the database with personal and contact information

**CSV**: Comma-Separated Values, plain text file format for data exchange

**Dashboard**: Main screen showing system overview and quick statistics

**Duplicate**: Two or more contacts with matching information (name, email, or phone)

**Export**: Save contacts from system to external file (Excel or CSV)

**Gallery**: Collection of photos for a specific contact

**Group**: Collection of users organized by function or department

**Import**: Load contacts into system from external file (Excel or CSV)

**LocalDB**: Local database engine that stores data on your computer

**Permission**: Authorization to perform specific action in system

**Profile Photo**: Main photo displayed for a contact in lists and details

**Role**: Predefined set of permissions (Administrator, Manager, User)

**Template**: Pre-formatted file structure for importing data correctly

**User Rights**: Individual permissions assigned to a specific user

### File Formats Supported

**Photos:**
- JPEG (.jpg, .jpeg)
- PNG (.png)
- GIF (.gif)

**Documents:**
- PDF (.pdf)
- Microsoft Word (.doc, .docx)
- Microsoft Excel (.xls, .xlsx)
- Text files (.txt)
- Images (all photo formats)

**Import/Export:**
- Excel 2007+ (.xlsx)
- CSV (.csv)

### Keyboard Shortcuts Reference

| Action | Shortcut |
|--------|----------|
| Save form | Ctrl + S |
| Cancel | Esc |
| Next field | Tab |
| Previous field | Shift + Tab |
| Submit form | Enter (when on button) |
| Go back | Backspace or Alt + ← |
| Refresh page | F5 or Ctrl + R |
| Search in page | Ctrl + F |

### Default Login Credentials

**Initial Setup:**
```
Username: admin
Password: Admin@123
```

⚠️ **IMPORTANT**: Change this password immediately after first login!

### Maximum Limits

| Item | Limit |
|------|-------|
| Photo file size | 5 MB |
| Document file size | 10 MB |
| Photos per contact | Unlimited (recommend 10) |
| Documents per contact | Unlimited (recommend 20) |
| Contacts in system | Unlimited (recommend under 10,000) |
| Import file rows | 10,000 rows |
| Export file rows | Unlimited |
| Username length | 3-20 characters |
| Password length | 6-50 characters |

### Version History

**Version 1.0.0** (February 2026)
- Initial release
- Contact management (CRUD operations)
- Photo upload and gallery
- Document attachment
- Import/Export (Excel/CSV)
- User and group management
- Role-based access control
- Duplicate detection
- Mobile-responsive interface

---

## End of Manual

Thank you for using Contact Management System!

**Document Version**: 1.0  
**Last Updated**: February 11, 2026  
**Format**: Complete User Manual  

For the latest version of this manual, check the application's Documentation folder.

**Questions?** Contact your system administrator.

---

*This manual covers all aspects of the Contact Management System. Bookmark this page for quick reference!*
