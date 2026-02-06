# API & Architecture Documentation

## Application Architecture

```
┌─────────────────────────────────────────────────────┐
│         Presentation Layer (Razor Views)             │
│  (HTML/CSS/JavaScript User Interface)                │
└─────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────┐
│      Business Logic Layer (Controllers)              │
│  (HomeController, PhotoController, DocumentController)
└─────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────┐
│      Services Layer (FileUploadService)              │
│  (Business operations and file handling)             │
└─────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────┐
│        Data Access Layer (Entity Framework)          │
│  (ApplicationDbContext, LINQ queries)                │
└─────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────┐
│           Database Layer (MS SQL Server)             │
│  (Tables: Contacts, Groups, Photos, Documents)      │
└─────────────────────────────────────────────────────┘
```

## Controller Endpoints

### HomeController

#### GET /
- **Description**: Display all contacts
- **Parameters**: 
  - `searchTerm` (optional): Filter contacts by name, email, or phone
- **Returns**: View with list of contacts

#### GET /home/details/{id}
- **Description**: View detailed contact information
- **Parameters**: 
  - `id`: Contact ID
- **Returns**: Detailed contact view with photos and documents

#### GET /home/create
- **Description**: Display form to create new contact
- **Returns**: Create form view

#### POST /home/create
- **Description**: Save new contact
- **Parameters**: Contact details in form body
- **Returns**: Redirect to contacts list

#### GET /home/edit/{id}
- **Description**: Display form to edit contact
- **Parameters**: 
  - `id`: Contact ID
- **Returns**: Edit form view

#### POST /home/edit/{id}
- **Description**: Update contact information
- **Parameters**: 
  - `id`: Contact ID
  - Contact details in form body
- **Returns**: Redirect to contact details

#### GET /home/delete/{id}
- **Description**: Display delete confirmation
- **Parameters**: 
  - `id`: Contact ID
- **Returns**: Delete confirmation view

#### POST /home/delete/{id}
- **Description**: Delete contact and related data
- **Parameters**: 
  - `id`: Contact ID
- **Returns**: Redirect to contacts list

### PhotoController

#### GET /photo/gallery/{id}
- **Description**: Display photo gallery for contact
- **Parameters**: 
  - `id`: Contact ID
- **Returns**: Gallery view with upload functionality

#### POST /photo/upload
- **Description**: Upload new photo for contact
- **Parameters**: 
  - `contactId`: Contact ID
  - `photoFile`: File to upload
- **Returns**: JSON response with upload status and photo path

#### POST /photo/SetProfilePhoto
- **Description**: Set a photo as contact profile photo
- **Parameters**: 
  - `contactId`: Contact ID
  - `photoId`: Photo ID
- **Returns**: JSON response with status

#### POST /photo/delete
- **Description**: Delete a photo
- **Parameters**: 
  - `id`: Photo ID
  - `contactId`: Contact ID
- **Returns**: JSON response with deletion status

### DocumentController

#### GET /document/list/{id}
- **Description**: Display documents for contact
- **Parameters**: 
  - `id`: Contact ID
- **Returns**: Document list view with upload form

#### POST /document/upload
- **Description**: Upload new document for contact
- **Parameters**: 
  - `contactId`: Contact ID
  - `documentFile`: File to upload
  - `documentType`: Type of document
- **Returns**: JSON response with upload status

#### POST /document/delete
- **Description**: Delete a document
- **Parameters**: 
  - `id`: Document ID
  - `contactId`: Contact ID
- **Returns**: JSON response with deletion status

#### GET /document/download/{id}/{contactId}
- **Description**: Download a document
- **Parameters**: 
  - `id`: Document ID
  - `contactId`: Contact ID
- **Returns**: File download

## Data Models

### Contact
```csharp
public class Contact
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string NickName { get; set; }
    public string Email { get; set; }
    public string Mobile1 { get; set; }
    public string Mobile2 { get; set; }
    public string Mobile3 { get; set; }
    public string WhatsAppNumber { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    public string PhotoPath { get; set; }
    public int? GroupId { get; set; }
    public ContactGroup Group { get; set; }
    public string OtherDetails { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<ContactPhoto> Photos { get; set; }
    public ICollection<ContactDocument> Documents { get; set; }
}
```

### ContactGroup
```csharp
public class ContactGroup
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<Contact> Contacts { get; set; }
}
```

### ContactPhoto
```csharp
public class ContactPhoto
{
    public int Id { get; set; }
    public int ContactId { get; set; }
    public Contact Contact { get; set; }
    public string PhotoPath { get; set; }
    public string FileName { get; set; }
    public long FileSize { get; set; }
    public string ContentType { get; set; }
    public bool IsProfilePhoto { get; set; }
    public DateTime UploadedAt { get; set; }
}
```

### ContactDocument
```csharp
public class ContactDocument
{
    public int Id { get; set; }
    public int ContactId { get; set; }
    public Contact Contact { get; set; }
    public string DocumentPath { get; set; }
    public string FileName { get; set; }
    public long FileSize { get; set; }
    public string ContentType { get; set; }
    public string DocumentType { get; set; }
    public DateTime UploadedAt { get; set; }
}
```

## Database Schema

### Relationships

```
ContactGroup (1) ──── (Many) Contact
Contact (1) ──── (Many) ContactPhoto
Contact (1) ──── (Many) ContactDocument
```

### SQL Queries

**Get all contacts with groups:**
```sql
SELECT c.*, cg.Name as GroupName
FROM Contacts c
LEFT JOIN ContactGroups cg ON c.GroupId = cg.Id
ORDER BY c.UpdatedAt DESC
```

**Search contacts:**
```sql
SELECT * FROM Contacts
WHERE FirstName LIKE '%searchTerm%'
   OR LastName LIKE '%searchTerm%'
   OR Email LIKE '%searchTerm%'
   OR Mobile1 LIKE '%searchTerm%'
   OR Mobile2 LIKE '%searchTerm%'
   OR Mobile3 LIKE '%searchTerm%'
```

**Get contact with all related data:**
```sql
SELECT c.*, cg.*, p.*, d.*
FROM Contacts c
LEFT JOIN ContactGroups cg ON c.GroupId = cg.Id
LEFT JOIN ContactPhotos p ON c.Id = p.ContactId
LEFT JOIN ContactDocuments d ON c.Id = d.ContactId
WHERE c.Id = @ContactId
```

## File Upload Service

### Features

- **File Validation**: Checks file size and extension
- **Secure Storage**: Files stored in wwwroot/uploads with timestamp naming
- **Error Handling**: Comprehensive error messages
- **Async Operations**: Non-blocking file uploads

### Configuration

```json
"FileUpload": {
  "MaxPhotoSize": 5242880,
  "MaxDocumentSize": 10485760,
  "AllowedPhotoExtensions": [ ".jpg", ".jpeg", ".png", ".gif", ".bmp" ],
  "AllowedDocumentExtensions": [ ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".ppt", ".pptx" ],
  "PhotoUploadPath": "uploads/photos",
  "DocumentUploadPath": "uploads/documents"
}
```

## Validation Rules

### Contact Fields
- **FirstName**: Required, max 100 characters
- **LastName**: Optional, max 100 characters
- **Email**: Optional, must be valid email format
- **Mobile Numbers**: Optional, standard phone format
- **Address**: Optional, max 500 characters

### File Uploads
- **Photos**: Max 5MB, JPG/PNG/GIF/BMP only
- **Documents**: Max 10MB, PDF/Office formats only

## Performance Considerations

### Database
- Indexes on FirstName, LastName, Email, Mobile1
- Lazy loading for related entities
- Pagination for large datasets (future enhancement)

### File Storage
- Organized by type (photos/documents)
- Unique filenames with timestamps
- Regular cleanup of orphaned files (future enhancement)

### Caching
- Can implement distributed caching for group lists
- Browser caching for static assets

## Error Handling

### Types
1. **Validation Errors**: Field validation failures
2. **Database Errors**: Connection, constraint violations
3. **File Errors**: Upload, size, permission issues
4. **Business Logic Errors**: Invalid operations

### Response Format

**Success Response:**
```json
{
  "success": true,
  "message": "Operation completed",
  "data": { }
}
```

**Error Response:**
```json
{
  "success": false,
  "message": "Error description"
}
```

## Security Features

### Current Implementation
- Input validation
- SQL injection protection (Entity Framework)
- CORS configuration
- Secure file handling

### Recommended Additions
- Authentication (ASP.NET Identity)
- Authorization (Role-based access)
- HTTPS enforcement
- CSRF protection (included in Razor)
- Rate limiting
- Data encryption

## Future API Enhancements

1. **REST API**
   - JSON endpoints for frontend frameworks
   - Swagger/OpenAPI documentation

2. **Pagination**
   - Limit/offset for large datasets
   - Sorting options

3. **Filtering**
   - Advanced search criteria
   - Date range filters

4. **Export**
   - CSV export
   - PDF export
   - vCard format

5. **Reporting**
   - Contact statistics
   - Activity logs
   - Audit trails

## Testing

### Unit Tests
```bash
dotnet test
```

### Integration Tests
- Database connection tests
- File upload tests
- CRUD operation tests

### Load Testing
- Contact search performance
- File upload performance
- Concurrent user handling

---

For deployment information, see [DEPLOYMENT.md](DEPLOYMENT.md).
For setup instructions, see [SETUP.md](SETUP.md).
