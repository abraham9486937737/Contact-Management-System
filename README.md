# Contact Management System

A comprehensive ASP.NET Core web application for managing personal and professional contacts with photo albums and document attachments.

## Features

- **Contact Management**
  - Add, edit, view, and delete contacts
  - Store comprehensive contact information
  - Organize contacts by groups (Family, Friends, Business, School, Church, Others)

- **Contact Information**
  - First Name, Last Name, Nick Name
  - Email Address
  - Multiple Phone Numbers (Mobile 1, 2, 3)
  - WhatsApp Number
  - Complete Address (Street, City, State, Postal Code, Country)
  - Additional Notes/Details

- **Photo Management**
  - Upload multiple photos for each contact
  - Photo gallery view
  - Set profile photo for contacts
  - Drag-and-drop upload support

- **Document Management**
  - Attach documents (PDF, Word, Excel, PowerPoint, etc.)
  - Categorize documents by type (ID, Address, Contract, etc.)
  - Download attached documents
  - Document metadata tracking

- **Search & Filter**
  - Search contacts by name, email, or phone number
  - View all contacts in an organized grid

- **Responsive UI**
  - Mobile-friendly design
  - Interactive and user-friendly interface
  - Modern styling with Font Awesome icons

## Technology Stack

- **Backend**: ASP.NET Core (C#)
- **Database**: MS SQL Server
- **Frontend**: HTML5, CSS3, JavaScript, Razor Pages
- **ORM**: Entity Framework Core
- **File Handling**: ASP.NET Core File Upload API

## Prerequisites

- .NET 8.0 SDK or later
- MS SQL Server (SQL Express or Full)
- Visual Studio 2022 (recommended) or VS Code

## Installation & Setup

### 1. Database Configuration

Update the connection string in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\SQLEXPRESS;Database=ContactManagementDB;Trusted_Connection=true;TrustServerCertificate=true;Encrypt=false;"
}
```

Replace `SQLEXPRESS` with your SQL Server instance name if different.

### 2. Apply Database Migrations

Navigate to the project directory and run:

```bash
dotnet ef database update
```

This will create the database and all necessary tables.

### 3. Build & Run

```bash
dotnet build
dotnet run
```

The application will be available at `https://localhost:5001` or `http://localhost:5000`

## Project Structure

```
ContactManagementAPI/
├── Models/                 # Data models
│   ├── Contact.cs
│   ├── ContactGroup.cs
│   ├── ContactPhoto.cs
│   └── ContactDocument.cs
├── Controllers/            # MVC Controllers
│   ├── HomeController.cs
│   ├── PhotoController.cs
│   └── DocumentController.cs
├── Data/                   # Database context
│   └── ApplicationDbContext.cs
├── Services/               # Business logic
│   └── FileUploadService.cs
├── Views/                  # Razor views
│   ├── Home/
│   ├── Photo/
│   ├── Document/
│   └── Shared/
├── wwwroot/               # Static files
│   ├── css/
│   ├── js/
│   └── uploads/           # User uploaded files
├── appsettings.json       # Configuration
└── Program.cs             # Application entry point
```

## Usage

### Adding a Contact

1. Click "Add New Contact" button
2. Fill in the contact details
3. Click "Create Contact"

### Managing Photos

1. Go to contact details
2. Click "Photo Gallery"
3. Drag-and-drop or click to select photos
4. Click "Set as Profile" to use as contact photo

### Managing Documents

1. Go to contact details
2. Click "Manage Documents"
3. Select document type and file
4. Click "Upload"
5. Use "Download" to retrieve documents

## Configuration Options

Edit `appsettings.json` to customize:

- File upload size limits
- Allowed file extensions
- Upload paths

```json
"FileUpload": {
  "MaxPhotoSize": 5242880,        // 5MB
  "MaxDocumentSize": 10485760,    // 10MB
  "AllowedPhotoExtensions": [ ".jpg", ".jpeg", ".png", ".gif", ".bmp" ],
  "AllowedDocumentExtensions": [ ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".ppt", ".pptx" ],
  "PhotoUploadPath": "uploads/photos",
  "DocumentUploadPath": "uploads/documents"
}
```

## Database Schema

### Contact Table
- Id (Primary Key)
- FirstName, LastName, NickName
- Email, Mobile1, Mobile2, Mobile3, WhatsAppNumber
- Address, City, State, PostalCode, Country
- PhotoPath, GroupId
- OtherDetails
- CreatedAt, UpdatedAt

### ContactGroup Table
- Id (Primary Key)
- Name, Description
- CreatedAt

### ContactPhoto Table
- Id (Primary Key)
- ContactId (Foreign Key)
- PhotoPath, FileName, FileSize, ContentType
- IsProfilePhoto
- UploadedAt

### ContactDocument Table
- Id (Primary Key)
- ContactId (Foreign Key)
- DocumentPath, FileName, FileSize, ContentType
- DocumentType
- UploadedAt

## Future Enhancements

- [ ] Export contacts to CSV/Excel
- [ ] vCard format import/export
- [ ] Contact groups management UI
- [ ] Birthday reminders
- [ ] Contact sharing (via links or email)
- [ ] Backup/Restore functionality
- [ ] Multi-user support with authentication
- [ ] Contact import from various sources
- [ ] Advanced search and filtering
- [ ] Contact merge functionality
- [ ] Activity timeline
- [ ] Mobile app version

## Security Considerations

- Implement authentication and authorization
- Add input validation and sanitization
- Use HTTPS in production
- Restrict file upload types and sizes
- Regular database backups
- Implement rate limiting for API endpoints

## Troubleshooting

### Database Connection Issues
- Verify SQL Server is running
- Check connection string in appsettings.json
- Ensure database user has appropriate permissions
- Try `Server=localhost\SQLEXPRESS` if instance name doesn't work

### File Upload Issues
- Check folder permissions for `wwwroot/uploads/`
- Verify file size limits in configuration
- Ensure allowed file extensions are configured

### Display Issues
- Clear browser cache (Ctrl+Shift+Delete)
- Check browser console for JavaScript errors
- Verify CSS file is loading (check browser DevTools)

## Contributing

Contributions are welcome! Please follow these guidelines:
1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## License

This project is licensed under the MIT License.

## Support

For issues, questions, or suggestions, please create an issue in the GitHub repository.

## Contact

Created with ❤️ for managing your valuable contacts!

---

**Last Updated**: February 2026
