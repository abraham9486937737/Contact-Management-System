using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContactManagementAPI.Data;
using ContactManagementAPI.Models;
using ContactManagementAPI.Services;
using ContactManagementAPI.Security;

namespace ContactManagementAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly FileUploadService _fileUploadService;
        private readonly ImportExportService _importExportService;
        private readonly ContactStatisticsService _statisticsService;

        public HomeController(ApplicationDbContext context, FileUploadService fileUploadService, ImportExportService importExportService, ContactStatisticsService statisticsService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
            _importExportService = importExportService;
            _statisticsService = statisticsService;
        }

        // GET: Home/Index - Display all contacts with search functionality
        [RequireRight(RightsCatalog.ContactsView)]
        public async Task<IActionResult> Index(string searchTerm = "")
        {
            var contacts = _context.Contacts
                .Include(c => c.Group)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                contacts = contacts.Where(c =>
                    c.FirstName.Contains(searchTerm) ||
                    (c.LastName != null && c.LastName.Contains(searchTerm)) ||
                    (c.Email != null && c.Email.Contains(searchTerm)) ||
                    (c.Mobile1 != null && c.Mobile1.Contains(searchTerm)) ||
                    (c.Mobile2 != null && c.Mobile2.Contains(searchTerm)) ||
                    (c.Mobile3 != null && c.Mobile3.Contains(searchTerm)));
            }

            ViewBag.SearchTerm = searchTerm;
            return View(await contacts.OrderByDescending(c => c.UpdatedAt).ToListAsync());
        }

        // GET: Home/Details/5
        [RequireRight(RightsCatalog.ContactsView)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var contact = await _context.Contacts
                .Include(c => c.Group)
                .Include(c => c.Photos)
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contact == null)
                return NotFound();

            return View(contact);
        }

        // GET: Home/Create
        [RequireRight(RightsCatalog.ContactsCreate)]
        public IActionResult Create()
        {
            ViewData["Groups"] = _context.ContactGroups.ToList();
            return View();
        }

        // POST: Home/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireRight(RightsCatalog.ContactsCreate)]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,NickName,Email,Mobile1,Mobile2,Mobile3,WhatsAppNumber,Address,City,State,PostalCode,Country,GroupId,OtherDetails")] Contact contact, IFormFile? profilePhoto)
        {
            if (ModelState.IsValid)
            {
                contact.CreatedAt = DateTime.Now;
                contact.UpdatedAt = DateTime.Now;
                
                // Save contact first to get the ID
                _context.Add(contact);
                await _context.SaveChangesAsync();
                
                // Handle profile photo upload after we have the contact ID
                if (profilePhoto != null)
                {
                    var result = await _fileUploadService.UploadPhotoAsync(profilePhoto, contact.Id);
                    if (result.Success)
                    {
                        contact.PhotoPath = result.FilePath;
                        _context.Update(contact);
                        await _context.SaveChangesAsync();
                    }
                }
                
                TempData["SuccessMessage"] = "Contact created successfully!";
                return RedirectToAction(nameof(Details), new { id = contact.Id });
            }
            ViewData["Groups"] = _context.ContactGroups.ToList();
            return View(contact);
        }

        // GET: Home/Edit/5
        [RequireRight(RightsCatalog.ContactsEdit)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
                return NotFound();

            ViewData["Groups"] = _context.ContactGroups.ToList();
            return View(contact);
        }

        // POST: Home/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireRight(RightsCatalog.ContactsEdit)]
        public async Task<IActionResult> Edit(int id, Contact contact, IFormFile? profilePhoto)
        {
            if (id != contact.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve existing contact to preserve navigation properties
                    var existingContact = await _context.Contacts.FindAsync(id);
                    if (existingContact == null)
                        return NotFound();

                    // Update properties
                    existingContact.FirstName = contact.FirstName;
                    existingContact.LastName = contact.LastName;
                    existingContact.NickName = contact.NickName;
                    existingContact.Email = contact.Email;
                    existingContact.Mobile1 = contact.Mobile1;
                    existingContact.Mobile2 = contact.Mobile2;
                    existingContact.Mobile3 = contact.Mobile3;
                    existingContact.WhatsAppNumber = contact.WhatsAppNumber;
                    existingContact.Address = contact.Address;
                    existingContact.City = contact.City;
                    existingContact.State = contact.State;
                    existingContact.PostalCode = contact.PostalCode;
                    existingContact.Country = contact.Country;
                    existingContact.GroupId = contact.GroupId;
                    existingContact.OtherDetails = contact.OtherDetails;
                    existingContact.UpdatedAt = DateTime.Now;

                    // Handle profile photo upload
                    if (profilePhoto != null)
                    {
                        var result = await _fileUploadService.UploadPhotoAsync(profilePhoto, existingContact.Id);
                        if (result.Success)
                        {
                            // Delete old photo if exists
                            if (!string.IsNullOrEmpty(existingContact.PhotoPath))
                            {
                                _fileUploadService.DeleteFile(existingContact.PhotoPath);
                            }
                            existingContact.PhotoPath = result.FilePath;
                        }
                    }
                    
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Contact updated successfully!";
                    return RedirectToAction(nameof(Details), new { id = existingContact.Id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.Id))
                        return NotFound();
                    throw;
                }
            }
            ViewData["Groups"] = _context.ContactGroups.ToList();
            return View(contact);
        }

        // GET: Home/Delete/5
        [RequireRight(RightsCatalog.ContactsDelete)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var contact = await _context.Contacts
                .Include(c => c.Group)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contact == null)
                return NotFound();

            return View(contact);
        }

        // POST: Home/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RequireRight(RightsCatalog.ContactsDelete)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Contact deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(int id)
        {
            return _context.Contacts.Any(e => e.Id == id);
        }

        #region Import/Export Actions

        // GET: Home/Dashboard - Display contact statistics
        [RequireRight(RightsCatalog.ContactsView)]
        public async Task<IActionResult> Dashboard()
        {
            var statistics = await _statisticsService.GetStatisticsAsync();
            return View(statistics);
        }

        // GET: Home/FindDuplicates - Display potential duplicate contacts
        [RequireRight(RightsCatalog.ContactsView)]
        public async Task<IActionResult> FindDuplicates()
        {
            var duplicates = await _statisticsService.FindDuplicatesAsync();
            return View(duplicates);
        }

        // GET: Home/Import - Display import page
        [RequireRight(RightsCatalog.ContactsCreate)]
        public IActionResult Import()
        {
            return View();
        }

        // POST: Home/ImportFile - Handle file import
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireRight(RightsCatalog.ContactsCreate)]
        public async Task<IActionResult> ImportFile(IFormFile file, string fileType)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a file to import.";
                return RedirectToAction(nameof(Import));
            }

            List<Contact> contacts;
            List<string> errors;

            try
            {
                using var stream = file.OpenReadStream();

                if (fileType == "excel")
                {
                    (contacts, errors) = await _importExportService.ImportFromExcel(stream);
                }
                else if (fileType == "csv")
                {
                    (contacts, errors) = await _importExportService.ImportFromCsv(stream);
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid file type selected.";
                    return RedirectToAction(nameof(Import));
                }

                if (errors.Any())
                {
                    TempData["ErrorMessage"] = $"Import completed with errors:<br/>{string.Join("<br/>", errors)}";
                }

                if (contacts.Any())
                {
                    // Set change tracking to true for saving
                    _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
                    
                    await _context.Contacts.AddRangeAsync(contacts);
                    await _context.SaveChangesAsync();
                    
                    // Reset tracking behavior
                    _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                    TempData["SuccessMessage"] = $"Successfully imported {contacts.Count} contact(s)!";
                }
                else
                {
                    TempData["ErrorMessage"] = "No valid contacts found in the file.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error importing file: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Home/ExportExcel - Export to Excel
        [RequireRight(RightsCatalog.ContactsView)]
        public async Task<IActionResult> ExportExcel()
        {
            var contacts = await _context.Contacts
                .Include(c => c.Group)
                .OrderBy(c => c.FirstName)
                .ToListAsync();

            var fileBytes = await _importExportService.ExportToExcel(contacts);
            var fileName = $"Contacts_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        // GET: Home/ExportCsv - Export to CSV
        [RequireRight(RightsCatalog.ContactsView)]
        public async Task<IActionResult> ExportCsv()
        {
            var contacts = await _context.Contacts
                .Include(c => c.Group)
                .OrderBy(c => c.FirstName)
                .ToListAsync();

            var fileBytes = await _importExportService.ExportToCsv(contacts);
            var fileName = $"Contacts_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            
            return File(fileBytes, "text/csv", fileName);
        }

        // GET: Home/ExportPdf - Export to PDF
        [RequireRight(RightsCatalog.ContactsView)]
        public async Task<IActionResult> ExportPdf()
        {
            var contacts = await _context.Contacts
                .Include(c => c.Group)
                .OrderBy(c => c.FirstName)
                .ToListAsync();

            var fileBytes = await _importExportService.ExportToPdf(contacts);
            var fileName = $"Contacts_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            
            return File(fileBytes, "application/pdf", fileName);
        }

        // GET: Home/DownloadTemplate - Download import template
        public async Task<IActionResult> DownloadTemplate(string type)
        {
            if (type == "excel")
            {
                var fileBytes = await _importExportService.GenerateExcelTemplate();
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Contact_Import_Template.xlsx");
            }
            else if (type == "csv")
            {
                var fileBytes = await _importExportService.GenerateCsvTemplate();
                return File(fileBytes, "text/csv", "Contact_Import_Template.csv");
            }

            return NotFound();
        }

        #endregion
    }
}
