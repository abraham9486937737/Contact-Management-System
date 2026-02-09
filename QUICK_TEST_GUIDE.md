# Quick Testing Reference - CRUD Operations

## 🚀 Quick Start Testing

### The application is now running at: **http://localhost:5000**

---

## 📋 Sample Data Available

| Contact | Email | Phone | Group | Documents |
|---------|-------|-------|-------|-----------|
| John Doe | john.doe@email.com | +1-555-0101 | Family | 2 |
| Sarah Smith | sarah.smith@email.com | +1-555-0201 | Friends | 2 |
| Michael Johnson | michael.j@email.com | +1-555-0301 | Business | 1 |
| Emily Brown | emily.brown@email.com | +1-555-0401 | Family | 1 |
| David Wilson | david.w@email.com | +1-555-0501 | School | 2 |

---

## 🔄 CRUD Operations - Quick Test

### ✅ READ (View All)
```
Click: Home / View Contacts
Expected: See all 5 contacts in a table
```

### ✅ READ (View One)
```
Click: Any contact name or "View Details"
Expected: See full info + photo + documents
```

### ✅ CREATE (Add)
```
Click: "Create New Contact" button
Fill: First Name (required), other fields optional
Upload: Profile photo (optional)
Click: "Create"
Expected: New contact appears in list
```

### ✅ UPDATE (Edit)
```
Click: "Edit" on any contact
Change: Any field (e.g., phone, address)
Upload: New photo (optional, replaces old)
Click: "Update"
Expected: Changes appear immediately
```

### ✅ DELETE (Remove)
```
Click: "Delete" on any contact
Confirm: "Delete" on confirmation page
Expected: Contact removed from list
```

---

## 🧪 Testing Checklist

### READ Operations
- [ ] See all 5 sample contacts
- [ ] Click on John Doe - see 2 documents
- [ ] Click on Sarah Smith - see 2 documents  
- [ ] Click on Michael Johnson - see 1 document
- [ ] Click on Emily Brown - see 1 document
- [ ] Click on David Wilson - see 2 documents

### CREATE Operations
- [ ] Create contact with all fields
- [ ] Create contact with minimal fields
- [ ] Upload profile photo
- [ ] Select different groups
- [ ] Verify in list

### UPDATE Operations
- [ ] Edit phone number
- [ ] Edit address
- [ ] Change group
- [ ] Update photo
- [ ] Verify changes

### DELETE Operations
- [ ] Delete one contact
- [ ] Verify it's gone
- [ ] Recreate it

---

## 📁 Files Location

**Sample Photos**: `ContactManagementAPI\wwwroot\uploads\photos\`
- 1_sample.jpg, 2_sample.jpg, 3_sample.jpg, 4_sample.jpg, 5_sample.jpg

**Sample Documents**: `ContactManagementAPI\wwwroot\uploads\documents\`
- Various PDF files for each contact

---

## 🔗 Direct URLs

| Action | URL |
|--------|-----|
| Home | http://localhost:5000/ |
| Create | http://localhost:5000/Home/Create |
| John Details | http://localhost:5000/Home/Details/1 |
| Sarah Details | http://localhost:5000/Home/Details/2 |
| Michael Details | http://localhost:5000/Home/Details/3 |
| Emily Details | http://localhost:5000/Home/Details/4 |
| David Details | http://localhost:5000/Home/Details/5 |

---

## 📊 Sample Data Summary

✓ 5 Complete Contacts
✓ 5 Profile Photos
✓ 8 Sample Documents
✓ All Linked Correctly
✓ Ready for Testing

---

## ✨ All CRUD Operations Status

| Operation | Status |
|-----------|--------|
| Create | ✅ WORKING |
| Read | ✅ WORKING |
| Update | ✅ WORKING |
| Delete | ✅ WORKING |
| Photos | ✅ WORKING |
| Documents | ✅ WORKING |
| Search | ✅ WORKING |
| Groups | ✅ WORKING |

---

## 🎯 What to Do Now

1. **Open browser** → http://localhost:5000
2. **See sample data** → Click "View Contacts"
3. **Test each contact** → Click on names to see details
4. **Create your own** → Click "Create New Contact"
5. **Edit/Delete** → Test modifications and removal
6. **Clean up** → Delete sample data when done

---

## ❌ Troubleshooting

**No contacts showing?**
- Refresh page (F5)
- Check database migration ran

**Photos not visible?**
- Check wwwroot/uploads/photos/ folder exists
- Refresh page

**Can't create contact?**
- Check FirstName is filled (required)
- Check form validation errors

---

## 📝 Notes

- FirstName is REQUIRED field
- All other fields are optional
- Photos and Documents are optional
- 6 Contact Groups available
- Database: SQL Server Express (Local)

---

**Application Status: ✅ READY FOR TESTING**

Start testing now at: **http://localhost:5000**
