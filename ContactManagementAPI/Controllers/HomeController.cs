using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContactManagementAPI.Data;
using ContactManagementAPI.Models;
using ContactManagementAPI.Services;
using ContactManagementAPI.Security;
using System.Globalization;
using Microsoft.AspNetCore.Diagnostics;

namespace ContactManagementAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly FileUploadService _fileUploadService;
        private readonly ImportExportService _importExportService;
        private readonly ContactStatisticsService _statisticsService;
        private readonly UserContextService _userContextService;
        private readonly AdminHistoryService _adminHistoryService;

        public HomeController(
            ApplicationDbContext context,
            FileUploadService fileUploadService,
            ImportExportService importExportService,
            ContactStatisticsService statisticsService,
            UserContextService userContextService,
            AdminHistoryService adminHistoryService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
            _importExportService = importExportService;
            _statisticsService = statisticsService;
            _userContextService = userContextService;
            _adminHistoryService = adminHistoryService;
        }

        // GET: Home/Index - Display all contacts with search functionality
        [RequireRight(RightsCatalog.ContactsView)]
        public async Task<IActionResult> Index(string searchTerm = "")
        {
            var currentUser = _userContextService.CurrentUser;
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var contacts = ApplyContactScope(
                    _context.Contacts
                        .Include(c => c.Group)
                        .AsQueryable(),
                    currentUser)
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

            var currentUser = _userContextService.CurrentUser;
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var contact = await _context.Contacts
                .Include(c => c.Group)
                .Include(c => c.Photos)
                .Include(c => c.Documents)
                .Include(c => c.BankAccounts)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contact == null)
                return NotFound();

            if (!CanAccessContact(currentUser, contact))
            {
                TempData["ErrorMessage"] = "You can view only contacts from your group.";
                return RedirectToAction(nameof(Index));
            }

            return View(contact);
        }

        // GET: Home/Create
        [RequireRight(RightsCatalog.ContactsCreate)]
        public IActionResult Create()
        {
            var currentUser = _userContextService.CurrentUser;
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            PopulateFormData();

            if (!currentUser.IsAdmin)
            {
                var scopedContactGroupId = ResolveContactGroupIdForUser(currentUser);
                if (scopedContactGroupId.HasValue)
                {
                    ViewData["ForcedGroupId"] = scopedContactGroupId.Value;
                }
            }

            return View();
        }

        // POST: Home/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireRight(RightsCatalog.ContactsCreate)]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,NickName,Gender,DateOfBirth,Email,Mobile1,Mobile2,Mobile3,WhatsAppNumber,PassportNumber,PanNumber,AadharNumber,DrivingLicenseNumber,VotersId,Address,City,State,PostalCode,Country,GroupId,OtherDetails")] Contact contact, List<ContactBankAccount>? bankAccounts, IFormFile? profilePhoto)
        {
            var currentUser = _userContextService.CurrentUser;
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!currentUser.IsAdmin)
            {
                var scopedContactGroupId = ResolveContactGroupIdForUser(currentUser);
                if (!scopedContactGroupId.HasValue)
                {
                    ModelState.AddModelError(nameof(Contact.GroupId), "Your account is not assigned to a contact group.");
                }
                else
                {
                    contact.GroupId = scopedContactGroupId.Value;
                }
            }

            NormalizeOptionalBankAccountModelState();
            ValidateDuplicateContact(contact);

            if (ModelState.IsValid)
            {
                contact.CreatedAt = DateTime.Now;
                contact.UpdatedAt = DateTime.Now;

                var preparedBankAccounts = PrepareBankAccounts(bankAccounts);
                SyncLegacyBankFields(contact, preparedBankAccounts.FirstOrDefault());
                
                // Save contact first to get the ID
                _context.Add(contact);
                await _context.SaveChangesAsync();

                if (preparedBankAccounts.Any())
                {
                    foreach (var bankAccount in preparedBankAccounts)
                    {
                        bankAccount.ContactId = contact.Id;
                    }

                    _context.ContactBankAccounts.AddRange(preparedBankAccounts);
                    await _context.SaveChangesAsync();
                }
                
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

                _adminHistoryService.Log(
                    actionType: "Create",
                    entityType: "Contact",
                    entityId: contact.Id,
                    performedBy: _userContextService.CurrentUser?.UserName ?? "Unknown",
                    details: $"Created contact '{contact.FirstName} {contact.LastName}'.");
                
                TempData["SuccessMessage"] = "Contact created successfully!";
                return RedirectToAction(nameof(Details), new { id = contact.Id });
            }
            PopulateFormData(contact, PrepareBankAccounts(bankAccounts));
            return View(contact);
        }

        // GET: Home/Edit/5
        [RequireRight(RightsCatalog.ContactsEdit)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var currentUser = _userContextService.CurrentUser;
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!currentUser.IsAdmin)
            {
                TempData["ErrorMessage"] = "Only admin can edit existing contacts.";
                return RedirectToAction(nameof(Index));
            }

            var contact = await _context.Contacts
                .Include(c => c.BankAccounts)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (contact == null)
                return NotFound();

            PopulateFormData(contact);
            return View(contact);
        }

        // POST: Home/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireRight(RightsCatalog.ContactsEdit)]
        public async Task<IActionResult> Edit(int id, List<ContactBankAccount>? bankAccounts, IFormFile? profilePhoto)
        {
            var currentUser = _userContextService.CurrentUser;
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!currentUser.IsAdmin)
            {
                TempData["ErrorMessage"] = "Only admin can edit existing contacts.";
                return RedirectToAction(nameof(Index));
            }

            NormalizeOptionalBankAccountModelState();

            var existingContact = await _context.Contacts
                .AsTracking()
                .Include(c => c.BankAccounts)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existingContact == null)
                return NotFound();

            var updateSucceeded = await TryUpdateModelAsync(
                existingContact,
                "",
                c => c.FirstName,
                c => c.LastName,
                c => c.NickName,
                c => c.Gender,
                c => c.DateOfBirth,
                c => c.Email,
                c => c.Mobile1,
                c => c.Mobile2,
                c => c.Mobile3,
                c => c.WhatsAppNumber,
                c => c.PassportNumber,
                c => c.PanNumber,
                c => c.AadharNumber,
                c => c.DrivingLicenseNumber,
                c => c.VotersId,
                c => c.Address,
                c => c.City,
                c => c.State,
                c => c.PostalCode,
                c => c.Country,
                c => c.GroupId,
                c => c.OtherDetails);

            if (!updateSucceeded)
            {
                PopulateFormData(existingContact, PrepareBankAccounts(bankAccounts));
                return View(existingContact);
            }

            ValidateDuplicateContact(existingContact, id);

            if (ModelState.IsValid)
            {
                try
                {
                    var postedGender = Request.Form["Gender"].ToString();
                    existingContact.Gender = string.IsNullOrWhiteSpace(postedGender) ? null : postedGender;

                    var postedDateOfBirth = Request.Form["DateOfBirth"].ToString();
                    if (string.IsNullOrWhiteSpace(postedDateOfBirth))
                    {
                        existingContact.DateOfBirth = null;
                    }
                    else if (DateTime.TryParseExact(postedDateOfBirth, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDateOfBirth) ||
                             DateTime.TryParse(postedDateOfBirth, out parsedDateOfBirth))
                    {
                        existingContact.DateOfBirth = parsedDateOfBirth;
                    }

                    existingContact.UpdatedAt = DateTime.Now;

                    var preparedBankAccounts = PrepareBankAccounts(bankAccounts);
                    if (existingContact.BankAccounts.Any())
                    {
                        _context.ContactBankAccounts.RemoveRange(existingContact.BankAccounts);
                    }

                    if (preparedBankAccounts.Any())
                    {
                        foreach (var bankAccount in preparedBankAccounts)
                        {
                            bankAccount.ContactId = existingContact.Id;
                        }

                        _context.ContactBankAccounts.AddRange(preparedBankAccounts);
                    }

                    SyncLegacyBankFields(existingContact, preparedBankAccounts.FirstOrDefault());

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

                    _adminHistoryService.Log(
                        actionType: "Edit",
                        entityType: "Contact",
                        entityId: existingContact.Id,
                        performedBy: _userContextService.CurrentUser?.UserName ?? "Unknown",
                        details: $"Edited contact '{existingContact.FirstName} {existingContact.LastName}'.");

                    TempData["SuccessMessage"] = "Contact updated successfully!";
                    return RedirectToAction(nameof(Details), new { id = existingContact.Id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(id))
                        return NotFound();
                    throw;
                }
            }
            PopulateFormData(existingContact, PrepareBankAccounts(bankAccounts));
            return View(existingContact);
        }

        // GET: Home/Delete/5
        [RequireRight(RightsCatalog.ContactsDelete)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var currentUser = _userContextService.CurrentUser;
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!string.Equals(currentUser.UserName, SeedData.SuperAdminUserName, StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Only Super Admin can delete contacts.";
                return RedirectToAction(nameof(Index));
            }

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
            var currentUser = _userContextService.CurrentUser;
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!string.Equals(currentUser.UserName, SeedData.SuperAdminUserName, StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Only Super Admin can delete contacts.";
                return RedirectToAction(nameof(Index));
            }

            var contact = await _context.Contacts.FindAsync(id);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Contact deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Home/DeleteMultiple - Bulk delete contacts
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireRight(RightsCatalog.ContactsDelete)]
        public async Task<IActionResult> DeleteMultiple(List<int> contactIds)
        {
            var currentUser = _userContextService.CurrentUser;
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!string.Equals(currentUser.UserName, SeedData.SuperAdminUserName, StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Only Super Admin can delete contacts.";
                return RedirectToAction(nameof(Index));
            }

            if (contactIds == null || !contactIds.Any())
            {
                TempData["ErrorMessage"] = "No contacts selected for deletion.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var contactsToDelete = await _context.Contacts
                    .Where(c => contactIds.Contains(c.Id))
                    .ToListAsync();

                if (contactsToDelete.Any())
                {
                    _context.Contacts.RemoveRange(contactsToDelete);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Successfully deleted {contactsToDelete.Count} contact(s)!";
                }
                else
                {
                    TempData["ErrorMessage"] = "No matching contacts found to delete.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting contacts: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(int id)
        {
            return _context.Contacts.Any(e => e.Id == id);
        }

        private void PopulateFormData(Contact? contact = null, List<ContactBankAccount>? bankAccounts = null)
        {
            ViewData["Groups"] = _context.ContactGroups.OrderBy(g => g.Name).ToList();

            var bankNames = _context.ContactBankAccounts
                .Where(b => !string.IsNullOrWhiteSpace(b.BankName))
                .Select(b => b.BankName!)
                .Distinct()
                .OrderBy(name => name)
                .ToList();

            if (!string.IsNullOrWhiteSpace(contact?.BankName) && !bankNames.Contains(contact.BankName))
            {
                bankNames.Add(contact.BankName);
                bankNames = bankNames.OrderBy(name => name).ToList();
            }

            ViewData["BankNames"] = bankNames;

            if (bankAccounts != null && bankAccounts.Any())
            {
                ViewData["BankAccounts"] = bankAccounts;
                return;
            }

            if (contact?.BankAccounts != null && contact.BankAccounts.Any())
            {
                ViewData["BankAccounts"] = contact.BankAccounts.OrderBy(b => b.Id).ToList();
                return;
            }

            if (contact != null && (!string.IsNullOrWhiteSpace(contact.BankAccountNumber) || !string.IsNullOrWhiteSpace(contact.BankName) || !string.IsNullOrWhiteSpace(contact.BranchName) || !string.IsNullOrWhiteSpace(contact.IfscCode)))
            {
                ViewData["BankAccounts"] = new List<ContactBankAccount>
                {
                    new ContactBankAccount
                    {
                        AccountNumber = contact.BankAccountNumber,
                        BankName = contact.BankName,
                        BranchName = contact.BranchName,
                        IfscCode = contact.IfscCode
                    }
                };
                return;
            }

            ViewData["BankAccounts"] = new List<ContactBankAccount> { new ContactBankAccount() };
        }

        private static List<ContactBankAccount> PrepareBankAccounts(List<ContactBankAccount>? bankAccounts)
        {
            return (bankAccounts ?? new List<ContactBankAccount>())
                .Where(b => !string.IsNullOrWhiteSpace(b.AccountNumber) || !string.IsNullOrWhiteSpace(b.BankName) || !string.IsNullOrWhiteSpace(b.BranchName) || !string.IsNullOrWhiteSpace(b.IfscCode))
                .Select(b => new ContactBankAccount
                {
                    AccountNumber = b.AccountNumber,
                    BankName = b.BankName,
                    BranchName = b.BranchName,
                    IfscCode = b.IfscCode,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                })
                .ToList();
        }

        private static void SyncLegacyBankFields(Contact contact, ContactBankAccount? primaryBankAccount)
        {
            if (primaryBankAccount == null)
            {
                contact.BankAccountNumber = null;
                contact.BankName = null;
                contact.BranchName = null;
                contact.IfscCode = null;
                return;
            }

            contact.BankAccountNumber = primaryBankAccount.AccountNumber;
            contact.BankName = primaryBankAccount.BankName;
            contact.BranchName = primaryBankAccount.BranchName;
            contact.IfscCode = primaryBankAccount.IfscCode;
        }

        private void NormalizeOptionalBankAccountModelState()
        {
            var bankAccountKeys = ModelState.Keys
                .Where(key => key.StartsWith("bankAccounts[", StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var key in bankAccountKeys)
            {
                ModelState.Remove(key);
            }
        }

        private void ValidateDuplicateContact(Contact contact, int? excludeContactId = null)
        {
            var normalizedMobile = NormalizeDigits(contact.Mobile1);
            var normalizedAadhar = NormalizeDigits(contact.AadharNumber);

            if (!string.IsNullOrWhiteSpace(normalizedMobile))
            {
                var mobileConflict = _context.Contacts
                    .AsNoTracking()
                    .Where(c => c.Id != (excludeContactId ?? 0) && !string.IsNullOrWhiteSpace(c.Mobile1))
                    .Select(c => new { c.FirstName, c.LastName, c.Mobile1 })
                    .ToList()
                    .FirstOrDefault(c => NormalizeDigits(c.Mobile1) == normalizedMobile);

                if (mobileConflict != null)
                {
                    ModelState.AddModelError(nameof(Contact.Mobile1),
                        $"Mobile number already exists for contact '{mobileConflict.FirstName} {mobileConflict.LastName}'.");
                }
            }

            if (!string.IsNullOrWhiteSpace(normalizedAadhar))
            {
                var aadharConflict = _context.Contacts
                    .AsNoTracking()
                    .Where(c => c.Id != (excludeContactId ?? 0) && !string.IsNullOrWhiteSpace(c.AadharNumber))
                    .Select(c => new { c.FirstName, c.LastName, c.AadharNumber })
                    .ToList()
                    .FirstOrDefault(c => NormalizeDigits(c.AadharNumber) == normalizedAadhar);

                if (aadharConflict != null)
                {
                    ModelState.AddModelError(nameof(Contact.AadharNumber),
                        $"Aadhar number already exists for contact '{aadharConflict.FirstName} {aadharConflict.LastName}'.");
                }
            }

            if (string.IsNullOrWhiteSpace(normalizedMobile) && string.IsNullOrWhiteSpace(normalizedAadhar))
            {
                var firstName = NormalizeText(contact.FirstName);
                var lastName = NormalizeText(contact.LastName);
                var nickName = NormalizeText(contact.NickName);

                if (!string.IsNullOrWhiteSpace(firstName))
                {
                    var nameConflict = _context.Contacts
                        .AsNoTracking()
                        .Where(c => c.Id != (excludeContactId ?? 0) && !string.IsNullOrWhiteSpace(c.FirstName))
                        .Select(c => new { c.FirstName, c.LastName, c.NickName })
                        .ToList()
                        .FirstOrDefault(c =>
                            NormalizeText(c.FirstName) == firstName &&
                            NormalizeText(c.LastName) == lastName &&
                            NormalizeText(c.NickName) == nickName);

                    if (nameConflict != null)
                    {
                        ModelState.AddModelError(nameof(Contact.FirstName),
                            "A contact with the same First Name, Last Name, and Nick Name already exists. Provide Mobile1 or Aadhar if this is a different person.");
                    }
                }
            }
        }

        private List<Contact> FilterDuplicateImportedContacts(List<Contact> contacts, List<string> errors)
        {
            var validContacts = new List<Contact>();

            var existingContacts = _context.Contacts
                .AsNoTracking()
                .Select(c => new { c.FirstName, c.LastName, c.NickName, c.Mobile1, c.AadharNumber })
                .ToList();

            var seenMobiles = new HashSet<string>();
            var seenAadhars = new HashSet<string>();
            var seenNames = new HashSet<string>();

            for (var i = 0; i < contacts.Count; i++)
            {
                var contact = contacts[i];
                var rowLabel = $"Row {i + 1} ({contact.FirstName} {contact.LastName})";

                var normalizedMobile = NormalizeDigits(contact.Mobile1);
                var normalizedAadhar = NormalizeDigits(contact.AadharNumber);
                var firstName = NormalizeText(contact.FirstName);
                var lastName = NormalizeText(contact.LastName);
                var nickName = NormalizeText(contact.NickName);
                var nameKey = $"{firstName}|{lastName}|{nickName}";

                var hasError = false;

                if (!string.IsNullOrWhiteSpace(normalizedMobile))
                {
                    var existsInDb = existingContacts.Any(c => NormalizeDigits(c.Mobile1) == normalizedMobile);
                    var existsInImport = seenMobiles.Contains(normalizedMobile);
                    if (existsInDb || existsInImport)
                    {
                        errors.Add($"{rowLabel}: Mobile1 already exists.");
                        hasError = true;
                    }
                }

                if (!string.IsNullOrWhiteSpace(normalizedAadhar))
                {
                    var existsInDb = existingContacts.Any(c => NormalizeDigits(c.AadharNumber) == normalizedAadhar);
                    var existsInImport = seenAadhars.Contains(normalizedAadhar);
                    if (existsInDb || existsInImport)
                    {
                        errors.Add($"{rowLabel}: AadharNumber already exists.");
                        hasError = true;
                    }
                }

                if (string.IsNullOrWhiteSpace(normalizedMobile) && string.IsNullOrWhiteSpace(normalizedAadhar))
                {
                    var existsInDb = existingContacts.Any(c =>
                        NormalizeText(c.FirstName) == firstName &&
                        NormalizeText(c.LastName) == lastName &&
                        NormalizeText(c.NickName) == nickName);
                    var existsInImport = seenNames.Contains(nameKey);

                    if (existsInDb || existsInImport)
                    {
                        errors.Add($"{rowLabel}: Same First Name + Last Name + Nick Name already exists.");
                        hasError = true;
                    }
                }

                if (hasError)
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(normalizedMobile))
                {
                    seenMobiles.Add(normalizedMobile);
                }

                if (!string.IsNullOrWhiteSpace(normalizedAadhar))
                {
                    seenAadhars.Add(normalizedAadhar);
                }

                if (string.IsNullOrWhiteSpace(normalizedMobile) && string.IsNullOrWhiteSpace(normalizedAadhar))
                {
                    seenNames.Add(nameKey);
                }

                validContacts.Add(contact);
            }

            return validContacts;
        }

        private static string NormalizeDigits(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return new string(value.Where(char.IsDigit).ToArray());
        }

        private static string NormalizeText(string? value)
        {
            return (value ?? string.Empty).Trim().ToUpperInvariant();
        }

        #region Import/Export Actions

        // GET: Home/Dashboard - Display contact statistics
        [RequireRight(RightsCatalog.ContactsView)]
        public async Task<IActionResult> Dashboard()
        {
            var currentUser = _userContextService.CurrentUser;
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!currentUser.IsAdmin)
            {
                TempData["ErrorMessage"] = "Dashboard is available only for admin.";
                return RedirectToAction(nameof(Index));
            }

            var statistics = await _statisticsService.GetStatisticsAsync();
            return View(statistics);
        }

        // GET: Home/FindDuplicates - Display potential duplicate contacts
        [RequireRight(RightsCatalog.ContactsView)]
        public async Task<IActionResult> FindDuplicates()
        {
            var currentUser = _userContextService.CurrentUser;
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!currentUser.IsAdmin)
            {
                TempData["ErrorMessage"] = "Find Duplicates is available only for admin.";
                return RedirectToAction(nameof(Index));
            }

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
            var currentUser = _userContextService.CurrentUser;
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

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
                    if (!currentUser.IsAdmin)
                    {
                        var scopedContactGroupId = ResolveContactGroupIdForUser(currentUser);
                        if (!scopedContactGroupId.HasValue)
                        {
                            TempData["ErrorMessage"] = "Your account is not assigned to a contact group.";
                            return RedirectToAction(nameof(Import));
                        }

                        foreach (var importedContact in contacts)
                        {
                            importedContact.GroupId = scopedContactGroupId.Value;
                        }
                    }

                    contacts = FilterDuplicateImportedContacts(contacts, errors);

                    if (errors.Any())
                    {
                        TempData["ErrorMessage"] = $"Import completed with errors:<br/>{string.Join("<br/>", errors)}";
                    }

                    if (!contacts.Any())
                    {
                        TempData["ErrorMessage"] = TempData["ErrorMessage"] ?? "No valid contacts found in the file.";
                        return RedirectToAction(nameof(Import));
                    }

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
            var currentUser = _userContextService.CurrentUser;
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var contacts = await ApplyContactScope(
                    _context.Contacts
                        .Include(c => c.Group)
                        .OrderBy(c => c.FirstName),
                    currentUser)
                .ToListAsync();

            var fileBytes = await _importExportService.ExportToExcel(contacts);
            var fileName = $"Contacts_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        // GET: Home/ExportCsv - Export to CSV
        [RequireRight(RightsCatalog.ContactsView)]
        public async Task<IActionResult> ExportCsv()
        {
            var currentUser = _userContextService.CurrentUser;
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var contacts = await ApplyContactScope(
                    _context.Contacts
                        .Include(c => c.Group)
                        .OrderBy(c => c.FirstName),
                    currentUser)
                .ToListAsync();

            var fileBytes = await _importExportService.ExportToCsv(contacts);
            var fileName = $"Contacts_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            
            return File(fileBytes, "text/csv", fileName);
        }

        // GET: Home/ExportPdf - Export to PDF
        [RequireRight(RightsCatalog.ContactsView)]
        public async Task<IActionResult> ExportPdf()
        {
            var currentUser = _userContextService.CurrentUser;
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var contacts = await ApplyContactScope(
                    _context.Contacts
                        .Include(c => c.Group)
                        .OrderBy(c => c.FirstName),
                    currentUser)
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

        private IQueryable<Contact> ApplyContactScope(IQueryable<Contact> query, AppUser currentUser)
        {
            if (currentUser.IsAdmin)
            {
                return query;
            }

            var scopedContactGroupId = ResolveContactGroupIdForUser(currentUser);
            if (!scopedContactGroupId.HasValue)
            {
                return query.Where(c => false);
            }

            var groupId = scopedContactGroupId.Value;
            return query.Where(c => c.GroupId == groupId);
        }

        private bool CanAccessContact(AppUser currentUser, Contact contact)
        {
            if (currentUser.IsAdmin)
            {
                return true;
            }

            var scopedContactGroupId = ResolveContactGroupIdForUser(currentUser);
            return scopedContactGroupId.HasValue && contact.GroupId == scopedContactGroupId.Value;
        }

        private int? ResolveContactGroupIdForUser(AppUser user)
        {
            if (user.IsAdmin)
            {
                return null;
            }

            if (user.GroupId <= 0)
            {
                return null;
            }

            var userGroupName = _context.UserGroups
                .Where(g => g.Id == user.GroupId)
                .Select(g => g.Name)
                .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(userGroupName) && userGroupName.StartsWith("ContactGroup - ", StringComparison.OrdinalIgnoreCase))
            {
                var contactGroupName = userGroupName.Substring("ContactGroup - ".Length).Trim();
                var mappedContactGroupId = _context.ContactGroups
                    .Where(cg => cg.Name == contactGroupName)
                    .Select(cg => (int?)cg.Id)
                    .FirstOrDefault();

                if (mappedContactGroupId.HasValue)
                {
                    return mappedContactGroupId.Value;
                }
            }

            var directMatch = _context.ContactGroups
                .Where(cg => cg.Id == user.GroupId)
                .Select(cg => (int?)cg.Id)
                .FirstOrDefault();

            return directMatch;
        }

        [HttpGet]
        public IActionResult Error()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var statusCode = Response?.StatusCode > 0 ? Response.StatusCode : StatusCodes.Status500InternalServerError;

            ViewData["StatusCode"] = statusCode;
            ViewData["Path"] = exceptionFeature?.Path ?? HttpContext.Request.Path.Value ?? "/";
            ViewData["ErrorMessage"] = exceptionFeature?.Error?.Message ?? "An unexpected error occurred.";

            return View();
        }

        #endregion
    }
}
