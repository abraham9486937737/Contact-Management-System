using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ContactManagementAPI.Services
{
    public class FileUploadService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private const long MAX_PHOTO_SIZE = 5242880; // 5MB
        private const long MAX_DOCUMENT_SIZE = 10485760; // 10MB

        public FileUploadService(IWebHostEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
        }

        private string GetUploadsRoot()
        {
            var uploadsRoot = Environment.GetEnvironmentVariable("UPLOADS_ROOT");
            if (!string.IsNullOrWhiteSpace(uploadsRoot))
            {
                Directory.CreateDirectory(uploadsRoot);
                return uploadsRoot;
            }

            return Path.Combine(_environment.WebRootPath, "uploads");
        }

        public async Task<(bool Success, string FilePath, string ErrorMessage)> UploadPhotoAsync(IFormFile file, int contactId)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return (false, "", "No file selected");

                if (file.Length > MAX_PHOTO_SIZE)
                    return (false, "", "Photo size exceeds maximum limit of 5MB");

                var allowedExtensions = _configuration.GetSection("FileUpload:AllowedPhotoExtensions").Get<string[]>();
                var fileExtension = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                    return (false, "", "Invalid file format. Allowed formats: JPG, PNG, GIF, BMP");

                var uploadPath = Path.Combine(GetUploadsRoot(), "photos");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var fileName = $"{contactId}_{DateTime.Now.Ticks}{fileExtension}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return (true, $"/uploads/photos/{fileName}", "");
            }
            catch (Exception ex)
            {
                return (false, "", $"Error uploading file: {ex.Message}");
            }
        }

        public async Task<(bool Success, string FilePath, string ErrorMessage)> UploadDocumentAsync(IFormFile file, int contactId)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return (false, "", "No file selected");

                if (file.Length > MAX_DOCUMENT_SIZE)
                    return (false, "", "Document size exceeds maximum limit of 10MB");

                var allowedExtensions = _configuration.GetSection("FileUpload:AllowedDocumentExtensions").Get<string[]>();
                var fileExtension = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                    return (false, "", "Invalid file format");

                var uploadPath = Path.Combine(GetUploadsRoot(), "documents");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var fileName = $"{contactId}_{DateTime.Now.Ticks}{fileExtension}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return (true, $"/uploads/documents/{fileName}", "");
            }
            catch (Exception ex)
            {
                return (false, "", $"Error uploading file: {ex.Message}");
            }
        }

        public bool DeleteFile(string filePath)
        {
            try
            {
                var trimmed = (filePath ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(trimmed))
                {
                    return false;
                }

                var normalized = trimmed.StartsWith("/") ? trimmed : "/" + trimmed;
                var uploadsRoot = GetUploadsRoot();
                string fullPath;

                if (normalized.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
                {
                    var relative = normalized.Substring("/uploads/".Length).Replace('/', Path.DirectorySeparatorChar);
                    fullPath = Path.Combine(uploadsRoot, relative);
                }
                else
                {
                    fullPath = Path.Combine(_environment.WebRootPath, normalized.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                }

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
