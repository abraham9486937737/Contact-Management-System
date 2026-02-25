using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContactManagementAPI.Data;
using ContactManagementAPI.Models;
using ContactManagementAPI.Services;
using ContactManagementAPI.Security;

namespace ContactManagementAPI.Controllers
{
    public class DocumentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly FileUploadService _fileUploadService;

        public DocumentController(ApplicationDbContext context, FileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        // GET: Document/List/5
        [RequireRight(RightsCatalog.ContactsView)]
        public async Task<IActionResult> List(int? id)
        {
            if (id == null)
                return NotFound();

            var contact = await _context.Contacts
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contact == null)
                return NotFound();

            return View(contact);
        }

        // POST: Document/Upload
        [HttpPost]
        [RequireRight(RightsCatalog.DocumentsManage)]
        public async Task<IActionResult> Upload(int contactId, IFormFile documentFile, string documentType = "Other")
        {
            var contact = await _context.Contacts.FindAsync(contactId);
            if (contact == null)
                return Json(new { success = false, message = "Contact not found" });

            var (success, filePath, errorMessage) = await _fileUploadService.UploadDocumentAsync(documentFile, contactId);
            if (!success)
                return Json(new { success = false, message = errorMessage });

            var document = new ContactDocument
            {
                ContactId = contactId,
                DocumentPath = filePath,
                FileName = documentFile.FileName,
                FileSize = documentFile.Length,
                ContentType = documentFile.ContentType,
                DocumentType = documentType,
                UploadedAt = DateTime.Now
            };

            _context.ContactDocuments.Add(document);
            await _context.SaveChangesAsync();

            return Json(new { success = true, documentId = document.Id, documentPath = filePath });
        }

        // POST: Document/Delete
        [HttpPost]
        [RequireRight(RightsCatalog.DocumentsManage)]
        public async Task<IActionResult> Delete(int id, int contactId)
        {
            var document = await _context.ContactDocuments.FirstOrDefaultAsync(d => d.Id == id && d.ContactId == contactId);
            if (document == null)
                return Json(new { success = false, message = "Document not found" });

            _fileUploadService.DeleteFile(document.DocumentPath);
            _context.ContactDocuments.Remove(document);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Document deleted" });
        }

        // GET: Document/Download
        [RequireRight(RightsCatalog.ContactsView)]
        public async Task<IActionResult> Download(int id, int contactId)
        {
            var document = await _context.ContactDocuments.FirstOrDefaultAsync(d => d.Id == id && d.ContactId == contactId);
            if (document == null)
                return NotFound();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", document.DocumentPath.TrimStart('/'));
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, document.ContentType, document.FileName);
        }
    }
}
