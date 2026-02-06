using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContactManagementAPI.Data;
using ContactManagementAPI.Models;
using ContactManagementAPI.Services;

namespace ContactManagementAPI.Controllers
{
    public class PhotoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly FileUploadService _fileUploadService;

        public PhotoController(ApplicationDbContext context, FileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        // GET: Photo/Gallery/5
        public async Task<IActionResult> Gallery(int? id)
        {
            if (id == null)
                return NotFound();

            var contact = await _context.Contacts
                .Include(c => c.Photos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contact == null)
                return NotFound();

            return View(contact);
        }

        // POST: Photo/Upload
        [HttpPost]
        public async Task<IActionResult> Upload(int contactId, IFormFile photoFile)
        {
            var contact = await _context.Contacts.FindAsync(contactId);
            if (contact == null)
                return Json(new { success = false, message = "Contact not found" });

            var (success, filePath, errorMessage) = await _fileUploadService.UploadPhotoAsync(photoFile, contactId);
            if (!success)
                return Json(new { success = false, message = errorMessage });

            var photo = new ContactPhoto
            {
                ContactId = contactId,
                PhotoPath = filePath,
                FileName = photoFile.FileName,
                FileSize = photoFile.Length,
                ContentType = photoFile.ContentType,
                IsProfilePhoto = false,
                UploadedAt = DateTime.Now
            };

            _context.ContactPhotos.Add(photo);
            await _context.SaveChangesAsync();

            return Json(new { success = true, photoId = photo.Id, photoPath = filePath });
        }

        // POST: Photo/SetProfilePhoto
        [HttpPost]
        public async Task<IActionResult> SetProfilePhoto(int contactId, int photoId)
        {
            var contact = await _context.Contacts
                .Include(c => c.Photos)
                .FirstOrDefaultAsync(c => c.Id == contactId);

            if (contact == null)
                return Json(new { success = false, message = "Contact not found" });

            var photo = await _context.ContactPhotos.FirstOrDefaultAsync(p => p.Id == photoId && p.ContactId == contactId);
            if (photo == null)
                return Json(new { success = false, message = "Photo not found" });

            // Unset all other profile photos
            foreach (var p in contact.Photos.Where(p => p.IsProfilePhoto))
            {
                p.IsProfilePhoto = false;
            }

            photo.IsProfilePhoto = true;
            contact.PhotoPath = photo.PhotoPath;
            _context.Update(contact);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Profile photo updated" });
        }

        // POST: Photo/Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id, int contactId)
        {
            var photo = await _context.ContactPhotos.FirstOrDefaultAsync(p => p.Id == id && p.ContactId == contactId);
            if (photo == null)
                return Json(new { success = false, message = "Photo not found" });

            _fileUploadService.DeleteFile(photo.PhotoPath);
            _context.ContactPhotos.Remove(photo);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Photo deleted" });
        }
    }
}
