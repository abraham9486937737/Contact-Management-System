# CRUD Operations Testing Guide

## Summary of Sample Data Added

The application now contains **5 sample contacts** with profile photos and documents for testing all CRUD operations:

### Sample Contacts:
1. **John Doe** (Family) - ID: 1
   - Email: john.doe@email.com
   - Mobile: +1-555-0101
   - Photo: ✓ (1_sample.jpg)
   - Documents: ID Proof, Resume (2 documents)

2. **Sarah Smith** (Friends) - ID: 2
   - Email: sarah.smith@email.com
   - Mobile: +1-555-0201
   - Photo: ✓ (2_sample.jpg)
   - Documents: Business Card, Address Proof (2 documents)

3. **Michael Johnson** (Business) - ID: 3
   - Email: michael.j@email.com
   - Mobile: +1-555-0301
   - Photo: ✓ (3_sample.jpg)
   - Documents: Contract (1 document)

4. **Emily Brown** (Family) - ID: 4
   - Email: emily.brown@email.com
   - Mobile: +1-555-0401
   - Photo: ✓ (4_sample.jpg)
   - Documents: ID Proof (1 document)

5. **David Wilson** (School) - ID: 5
   - Email: david.w@email.com
   - Mobile: +1-555-0501
   - Photo: ✓ (5_sample.jpg)
   - Documents: Certification, License (2 documents)

---

## CRUD Operations Testing Guide

### 1. **READ (View All Contacts)** ✓
- **URL**: `http://localhost:5000/` or `http://localhost:5000/Home/Index`
- **What to Test**:
  - You should see all 5 sample contacts listed
  - Each contact shows: Name, Email, Mobile, and Group
  - Contacts are sorted by Updated Date (newest first)
- **Expected Result**: All 5 sample contacts displayed in a table

### 2. **READ (View Single Contact Details)** ✓
- **URL**: `http://localhost:5000/Home/Details/1` (or any ID 1-5)
- **What to Test**:
  - Click on any contact name or "View Details" button
  - You should see:
    - Full contact information (all fields)
    - Profile photo displayed (if available)
    - All associated documents listed with download links
    - All associated photos in gallery view
  - Try IDs: 1, 2, 3, 4, 5

- **Example URLs**:
  - Details for John Doe: `http://localhost:5000/Home/Details/1`
  - Details for Sarah Smith: `http://localhost:5000/Home/Details/2`
  - Details for Michael Johnson: `http://localhost:5000/Home/Details/3`

### 3. **CREATE (Add New Contact)** ✓
- **URL**: `http://localhost:5000/Home/Create`
- **What to Test**:
  1. Click "Create New Contact" button on the index page
  2. Fill in the form:
     - First Name: Required field
     - Last Name, Nick Name, Email, Mobile numbers (optional)
     - Contact Group: Select from dropdown (Family, Friends, Business, School, Church, Others)
     - Address details (optional)
     - Upload a profile photo (optional)
     - Add other details (optional)
  3. Click "Create" button
  
- **Expected Result**:
  - New contact is saved to database
  - You are redirected to the Details page of the newly created contact
  - If you upload a photo, it appears on the Details page

### 4. **UPDATE (Edit Contact)** ✓
- **URL**: `http://localhost:5000/Home/Edit/1` (or any existing ID)
- **What to Test**:
  1. Click "Edit" button on any contact's details page
  2. Modify any field (e.g., change phone number, update address)
  3. Optionally upload a new profile photo
  4. Click "Update" button
  
- **Expected Result**:
  - Changes are saved to database
  - Updated timestamp is refreshed
  - If you upload a new photo, the old one is replaced
  - You are redirected to the updated contact's details page

### 5. **DELETE (Remove Contact)** ✓
- **URL**: `http://localhost:5000/Home/Delete/1` (or any existing ID)
- **What to Test**:
  1. Click "Delete" button on any contact
  2. Confirm deletion on the delete confirmation page
  3. Click "Delete" to confirm
  
- **Expected Result**:
  - Contact and all associated data (photos, documents) are removed
  - You are redirected to the contact list
  - Deleted contact no longer appears in the list

---

## Testing Checklist

### ✅ READ Operations
- [ ] View all contacts list
- [ ] View John Doe details (ID: 1) - with photo and 2 documents
- [ ] View Sarah Smith details (ID: 2) - with photo and 2 documents
- [ ] View Michael Johnson details (ID: 3) - with photo and 1 document
- [ ] View Emily Brown details (ID: 4) - with photo and 1 document
- [ ] View David Wilson details (ID: 5) - with photo and 2 documents

### ✅ CREATE Operations
- [ ] Create new contact with all fields filled
- [ ] Create contact with only required fields
- [ ] Upload profile photo during creation
- [ ] Select different contact groups
- [ ] Verify new contact appears in list

### ✅ UPDATE Operations
- [ ] Edit contact name
- [ ] Edit contact phone numbers
- [ ] Edit contact address
- [ ] Change contact group
- [ ] Update profile photo
- [ ] Verify changes saved

### ✅ DELETE Operations
- [ ] Delete a contact
- [ ] Confirm it no longer appears in the list
- [ ] Verify documents are also deleted

### ✅ Additional Features to Test
- [ ] Search contacts by name
- [ ] Search contacts by email
- [ ] Search contacts by phone number
- [ ] View documents (PDF files)
- [ ] View contact photos in gallery

---

## Files Location

### Photo Files:
```
ContactManagementAPI\wwwroot\uploads\photos\
├── 1_sample.jpg (John Doe)
├── 2_sample.jpg (Sarah Smith)
├── 3_sample.jpg (Michael Johnson)
├── 4_sample.jpg (Emily Brown)
└── 5_sample.jpg (David Wilson)
```

### Document Files:
```
ContactManagementAPI\wwwroot\uploads\documents\
├── 1_ID_Proof.pdf (John Doe)
├── 1_Resume.pdf (John Doe)
├── 2_Business_Card.pdf (Sarah Smith)
├── 2_Address_Proof.pdf (Sarah Smith)
├── 3_Contract.pdf (Michael Johnson)
├── 4_ID_Proof.pdf (Emily Brown)
├── 5_Certification.pdf (David Wilson)
└── 5_License.pdf (David Wilson)
```

---

## To Remove Sample Data

When you're ready to remove the sample contacts and use the application with your own data:

### Option 1: Delete via UI
Simply click the Delete button for each sample contact and confirm deletion.

### Option 2: Reset Database
To completely reset and remove sample data:

```powershell
cd e:\Contact_Management_System\ContactManagementAPI

# Remove the sample data migration
dotnet ef migrations remove

# Recreate the database without sample data
dotnet ef database drop --force
dotnet ef database update
```

---

## Troubleshooting

### If you can't see the photos:
- Ensure the upload directories exist:
  - `ContactManagementAPI\wwwroot\uploads\photos\`
  - `ContactManagementAPI\wwwroot\uploads\documents\`

### If new contact creation doesn't work:
1. Check that First Name is filled (it's required)
2. Check browser console for validation errors
3. Ensure form is submitted correctly (CSRF token should be auto-generated)

### If you get database errors:
- The migration has already been applied
- Database should contain all sample data
- Check `ContactManagementDB` in SQL Server Express

---

## Contact Management Features Overview

### Contact Fields:
- **First Name** (Required)
- **Last Name, Nick Name** (Optional)
- **Email** (Optional)
- **Mobile Numbers** (3 optional fields + 1 WhatsApp field)
- **Address Details** (City, State, Postal Code, Country)
- **Profile Photo** (Optional)
- **Contact Group** (6 default groups available)
- **Other Details** (Notes/Comments)

### Contact Groups:
1. Family
2. Friends
3. Business
4. School
5. Church
6. Others

---

## Application URLs

| Operation | URL |
|-----------|-----|
| View All Contacts | http://localhost:5000/ |
| Create Contact | http://localhost:5000/Home/Create |
| View Contact Details | http://localhost:5000/Home/Details/1 |
| Edit Contact | http://localhost:5000/Home/Edit/1 |
| Delete Contact | http://localhost:5000/Home/Delete/1 |

Replace `1` with any contact ID (1-5 for sample data).

---

## Next Steps

1. **Test the application** using the checklist above
2. **Verify all CRUD operations work** with the sample data
3. **Create your own contacts** using the Create form
4. **Remove sample data** when ready for production use
5. **Upload your profile photos and documents** for each contact

The application is now ready for full testing! 🎉
