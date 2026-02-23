using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContactManagementAPI.Data;
using ContactManagementAPI.Models;
using ContactManagementAPI.Services;
using ContactManagementAPI.Security;
using System.Text.Json;

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
        [RequireRight(RightsCatalog.PhotosManage)]
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
        [RequireRight(RightsCatalog.PhotosManage)]
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
        [RequireRight(RightsCatalog.PhotosManage)]
        public async Task<IActionResult> SetProfilePhoto(int contactId, int photoId)
        {
            (contactId, photoId) = await ResolveContactAndPhotoIdsAsync(contactId, photoId);

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
        [RequireRight(RightsCatalog.PhotosManage)]
        public async Task<IActionResult> Delete(int id, int contactId)
        {
            (contactId, id) = await ResolveContactAndPhotoIdsAsync(contactId, id);

            var photo = await _context.ContactPhotos.FirstOrDefaultAsync(p => p.Id == id && p.ContactId == contactId);
            if (photo == null)
            {
                photo = await _context.ContactPhotos.FirstOrDefaultAsync(p => p.Id == id);
            }

            if (photo == null)
                return Json(new { success = false, message = "Photo not found" });

            _fileUploadService.DeleteFile(photo.PhotoPath);
            _context.ContactPhotos.Remove(photo);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Photo deleted" });
        }

        private async Task<(int contactId, int photoId)> ResolveContactAndPhotoIdsAsync(int contactId, int photoId)
        {
            if (Request.HasFormContentType)
            {
                if (contactId <= 0 && int.TryParse(Request.Form["contactId"], out var formContactId))
                    contactId = formContactId;

                if (photoId <= 0)
                {
                    if (int.TryParse(Request.Form["photoId"], out var formPhotoId))
                        photoId = formPhotoId;
                    else if (int.TryParse(Request.Form["id"], out var formId))
                        photoId = formId;
                }
            }

            if (contactId <= 0 && int.TryParse(Request.Query["contactId"], out var queryContactId))
                contactId = queryContactId;

            if (photoId <= 0)
            {
                if (int.TryParse(Request.Query["photoId"], out var queryPhotoId))
                    photoId = queryPhotoId;
                else if (int.TryParse(Request.Query["id"], out var queryId))
                    photoId = queryId;
            }

            if ((contactId <= 0 || photoId <= 0) && Request.ContentType?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true)
            {
                Request.EnableBuffering();
                Request.Body.Position = 0;

                using var document = await JsonDocument.ParseAsync(Request.Body);
                var root = document.RootElement;

                if (contactId <= 0 && root.TryGetProperty("contactId", out var jsonContactId) && jsonContactId.TryGetInt32(out var parsedContactId))
                    contactId = parsedContactId;

                if (photoId <= 0)
                {
                    if (root.TryGetProperty("photoId", out var jsonPhotoId) && jsonPhotoId.TryGetInt32(out var parsedPhotoId))
                        photoId = parsedPhotoId;
                    else if (root.TryGetProperty("id", out var jsonId) && jsonId.TryGetInt32(out var parsedId))
                        photoId = parsedId;
                }

                Request.Body.Position = 0;
            }

            return (contactId, photoId);
        }
    }
}
