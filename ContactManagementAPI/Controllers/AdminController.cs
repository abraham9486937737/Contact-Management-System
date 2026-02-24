using System;
using System.Linq;
using System.IO.Compression;
using Microsoft.Data.Sqlite;
using ContactManagementAPI.Data;
using ContactManagementAPI.Models;
using ContactManagementAPI.Security;
using ContactManagementAPI.Services;
using ContactManagementAPI.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContactManagementAPI.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserContextService _userContextService;
        private readonly AdminHistoryService _adminHistoryService;
        private readonly IWebHostEnvironment _environment;
        private readonly PasswordHasher<AppUser> _passwordHasher = new();

        public AdminController(ApplicationDbContext context, UserContextService userContextService, AdminHistoryService adminHistoryService, IWebHostEnvironment environment)
        {
            _context = context;
            _userContextService = userContextService;
            _adminHistoryService = adminHistoryService;
            _environment = environment;
        }

        private static bool IsSuperAdminUser(AppUser? user)
        {
            return user != null && string.Equals(user.UserName, SeedData.SuperAdminUserName, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsProtectedSystemUser(AppUser? user)
        {
            return user != null &&
                   (IsSuperAdminUser(user) || string.Equals(user.UserName, "admin", StringComparison.OrdinalIgnoreCase));
        }

        private ContactGroup? ResolveContactGroupForUserGroup(UserGroup? userGroup)
        {
            if (userGroup == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(userGroup.Name) && userGroup.Name.StartsWith("ContactGroup - ", StringComparison.OrdinalIgnoreCase))
            {
                var contactGroupName = userGroup.Name.Substring("ContactGroup - ".Length).Trim();
                if (!string.IsNullOrWhiteSpace(contactGroupName))
                {
                    return _context.ContactGroups.FirstOrDefault(cg => cg.Name == contactGroupName);
                }
            }

            return _context.ContactGroups.FirstOrDefault(cg => cg.Id == userGroup.Id);
        }

        private string GetUploadsRoot()
        {
            var uploadsRoot = Environment.GetEnvironmentVariable("UPLOADS_ROOT");
            if (!string.IsNullOrWhiteSpace(uploadsRoot))
            {
                Directory.CreateDirectory(uploadsRoot);
                Directory.CreateDirectory(Path.Combine(uploadsRoot, "photos"));
                Directory.CreateDirectory(Path.Combine(uploadsRoot, "documents"));
                return uploadsRoot;
            }

            var fallback = Path.Combine(_environment.WebRootPath, "uploads");
            Directory.CreateDirectory(fallback);
            Directory.CreateDirectory(Path.Combine(fallback, "photos"));
            Directory.CreateDirectory(Path.Combine(fallback, "documents"));
            return fallback;
        }

        private static string NormalizeText(string? value)
        {
            return (value ?? string.Empty).Trim().ToUpperInvariant();
        }

        [RequireRight(RightsCatalog.AdminManageUsers)]
        public IActionResult Users()
        {
            var currentUser = _userContextService.CurrentUser;
            var isSuperAdmin = IsSuperAdminUser(currentUser);

            var usersQuery = _context.AppUsers
                .Include(u => u.Group)
                .AsQueryable();

            if (!isSuperAdmin)
            {
                usersQuery = usersQuery.Where(u => !string.Equals(u.UserName, SeedData.SuperAdminUserName, StringComparison.OrdinalIgnoreCase));
            }

            var users = usersQuery
                .OrderBy(u => u.UserName)
                .ToList();

            ViewBag.IsSuperAdmin = isSuperAdmin;

            return View(users);
        }

        [RequireRight(RightsCatalog.AdminManageUsers)]
        public IActionResult CreateUser()
        {
            ViewData["Groups"] = _context.UserGroups.OrderBy(g => g.Name).ToList();
            return View(new UserCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireRight(RightsCatalog.AdminManageUsers)]
        public IActionResult CreateUser(UserCreateViewModel model)
        {
            var currentUser = _userContextService.CurrentUser;
            var isSuperAdmin = IsSuperAdminUser(currentUser);

            if (string.Equals(model.UserName, SeedData.SuperAdminUserName, StringComparison.OrdinalIgnoreCase) && !isSuperAdmin)
            {
                ModelState.AddModelError(nameof(UserCreateViewModel.UserName), "Super Admin user name is reserved.");
            }

            if (_context.AppUsers.Any(u => u.UserName == model.UserName))
            {
                ModelState.AddModelError(nameof(UserCreateViewModel.UserName), "User name already exists.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["Groups"] = _context.UserGroups.OrderBy(g => g.Name).ToList();
                return View(model);
            }

            var user = new AppUser
            {
                UserName = model.UserName,
                FullName = model.FullName,
                GroupId = model.GroupId,
                IsAdmin = model.IsAdmin,
                IsActive = model.IsActive,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
            _context.AppUsers.Add(user);
            _context.SaveChanges();

            _adminHistoryService.Log(
                actionType: "Create",
                entityType: "User",
                entityId: user.Id,
                performedBy: _userContextService.CurrentUser?.UserName ?? "Unknown",
                details: $"Created user '{user.UserName}' in group id '{user.GroupId}'.");

            TempData["SuccessMessage"] = "User created successfully.";
            return RedirectToAction("Users");
        }

        [RequireRight(RightsCatalog.AdminManageUsers)]
        public IActionResult EditUser(int id)
        {
            var currentUser = _userContextService.CurrentUser;
            var isSuperAdmin = IsSuperAdminUser(currentUser);

            var user = _context.AppUsers.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound();

            if (IsSuperAdminUser(user) && !isSuperAdmin)
            {
                return NotFound();
            }

            var model = new UserEditViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                GroupId = user.GroupId,
                IsAdmin = user.IsAdmin,
                IsActive = user.IsActive
            };

            ViewData["Groups"] = _context.UserGroups.OrderBy(g => g.Name).ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireRight(RightsCatalog.AdminManageUsers)]
        public IActionResult EditUser(UserEditViewModel model)
        {
            var currentUser = _userContextService.CurrentUser;
            var isSuperAdmin = IsSuperAdminUser(currentUser);

            var user = _context.AppUsers.FirstOrDefault(u => u.Id == model.Id);
            if (user == null)
                return NotFound();

            if (IsSuperAdminUser(user) && !isSuperAdmin)
            {
                return NotFound();
            }

            if (_context.AppUsers.Any(u => u.UserName == model.UserName && u.Id != model.Id))
            {
                ModelState.AddModelError(nameof(UserEditViewModel.UserName), "User name already exists.");
            }

            if (string.Equals(model.UserName, SeedData.SuperAdminUserName, StringComparison.OrdinalIgnoreCase) && !IsSuperAdminUser(user))
            {
                ModelState.AddModelError(nameof(UserEditViewModel.UserName), "Super Admin user name is reserved.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["Groups"] = _context.UserGroups.OrderBy(g => g.Name).ToList();
                return View(model);
            }

            var wasAdminUser = string.Equals(user.UserName, "admin", StringComparison.OrdinalIgnoreCase);
            var wasSuperAdminUser = IsSuperAdminUser(user);

            if (wasSuperAdminUser)
            {
                // Super Admin is immutable (prevents lockout and prevents admin-level changes even by mistake)
                ModelState.AddModelError(string.Empty, "Super Admin account cannot be edited.");
                ViewData["Groups"] = _context.UserGroups.OrderBy(g => g.Name).ToList();
                return View(model);
            }

            user.UserName = model.UserName;
            user.FullName = model.FullName;
            user.GroupId = model.GroupId;
            user.IsAdmin = model.IsAdmin;
            user.IsActive = model.IsActive;
            user.UpdatedAt = DateTime.Now;

            if (wasAdminUser)
            {
                var administratorsGroupId = _context.UserGroups
                    .Where(g => g.Name == "Administrators")
                    .Select(g => g.Id)
                    .FirstOrDefault();

                user.IsAdmin = true;
                user.IsActive = true;
                if (administratorsGroupId > 0)
                {
                    user.GroupId = administratorsGroupId;
                }
            }

            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
            }

            _context.SaveChanges();

            _adminHistoryService.Log(
                actionType: "Edit",
                entityType: "User",
                entityId: user.Id,
                performedBy: _userContextService.CurrentUser?.UserName ?? "Unknown",
                details: $"Edited user '{user.UserName}' (Active: {user.IsActive}, Admin: {user.IsAdmin}).");

            TempData["SuccessMessage"] = "User updated successfully.";
            return RedirectToAction("Users");
        }

        [RequireRight(RightsCatalog.AdminManageRights)]
        public IActionResult UserRights(int id)
        {
            var currentUser = _userContextService.CurrentUser;
            var isSuperAdmin = IsSuperAdminUser(currentUser);

            var user = _context.AppUsers
                .Include(u => u.Group)
                .FirstOrDefault(u => u.Id == id);

            if (user == null)
                return NotFound();

            if (IsSuperAdminUser(user) && !isSuperAdmin)
            {
                return NotFound();
            }

            var userRights = _context.UserRights
                .Where(r => r.AppUserId == id)
                .ToList();

            var groupRights = _context.GroupRights
                .Where(r => r.UserGroupId == user.GroupId)
                .ToList();

            var rights = RightsCatalog.All.Select(r =>
            {
                var userRight = userRights.FirstOrDefault(ur => ur.RightKey == r.Key);
                var groupRight = groupRights.FirstOrDefault(gr => gr.RightKey == r.Key);
                var selection = userRight == null ? "Inherit" : (userRight.IsGranted ? "Grant" : "Deny");
                var effectiveGranted = user.IsAdmin || (userRight?.IsGranted ?? (groupRight?.IsGranted ?? false));
                var effectiveSource = user.IsAdmin ? "Admin" : userRight != null ? "User" : groupRight != null ? "Group" : "None";

                return new RightAssignmentViewModel
                {
                    Key = r.Key,
                    Label = r.Label,
                    Category = r.Category,
                    Selection = selection,
                    EffectiveGranted = effectiveGranted,
                    EffectiveSource = effectiveSource
                };
            }).ToList();

            var model = new UserRightsViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                GroupName = user.Group?.Name,
                Rights = rights
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireRight(RightsCatalog.AdminManageRights)]
        public IActionResult UserRights(UserRightsViewModel model)
        {
            var currentUser = _userContextService.CurrentUser;
            var isSuperAdmin = IsSuperAdminUser(currentUser);

            var user = _context.AppUsers.FirstOrDefault(u => u.Id == model.UserId);
            if (user == null)
                return NotFound();

            if (IsSuperAdminUser(user) && !isSuperAdmin)
            {
                return NotFound();
            }

            foreach (var right in model.Rights)
            {
                var existing = _context.UserRights.FirstOrDefault(r => r.AppUserId == model.UserId && r.RightKey == right.Key);
                if (right.Selection == "Inherit")
                {
                    if (existing != null)
                        _context.UserRights.Remove(existing);
                    continue;
                }

                var isGranted = right.Selection == "Grant";
                if (existing == null)
                {
                    _context.UserRights.Add(new UserRight
                    {
                        AppUserId = model.UserId,
                        RightKey = right.Key,
                        IsGranted = isGranted
                    });
                }
                else
                {
                    existing.IsGranted = isGranted;
                }
            }

            _context.SaveChanges();

            _adminHistoryService.Log(
                actionType: "Edit",
                entityType: "UserRights",
                entityId: model.UserId,
                performedBy: _userContextService.CurrentUser?.UserName ?? "Unknown",
                details: $"Updated rights for user '{user.UserName}'.");

            TempData["SuccessMessage"] = "User rights updated successfully.";
            return RedirectToAction("UserRights", new { id = model.UserId });
        }

        [RequireRight(RightsCatalog.AdminManageGroups)]
        public IActionResult Groups()
        {
            var groups = _context.UserGroups
                .OrderBy(g => g.Name)
                .ToList();

            ViewBag.IsSuperAdmin = IsSuperAdminUser(_userContextService.CurrentUser);

            return View(groups);
        }

        [RequireRight(RightsCatalog.AdminManageGroups)]
        public IActionResult CreateGroup()
        {
            return View(new GroupEditViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireRight(RightsCatalog.AdminManageGroups)]
        public IActionResult CreateGroup(GroupEditViewModel model)
        {
            if (_context.UserGroups.Any(g => g.Name == model.Name))
            {
                ModelState.AddModelError(nameof(GroupEditViewModel.Name), "Group name already exists.");
            }

            if (!ModelState.IsValid)
                return View(model);

            var group = new UserGroup
            {
                Name = model.Name,
                Description = model.Description,
                CreatedAt = DateTime.Now
            };

            _context.UserGroups.Add(group);
            _context.SaveChanges();

            var rights = RightsCatalog.All.Select(r => new GroupRight
            {
                UserGroupId = group.Id,
                RightKey = r.Key,
                IsGranted = false
            }).ToList();

            _context.GroupRights.AddRange(rights);
            _context.SaveChanges();

            _adminHistoryService.Log(
                actionType: "Create",
                entityType: "Group",
                entityId: group.Id,
                performedBy: _userContextService.CurrentUser?.UserName ?? "Unknown",
                details: $"Created group '{group.Name}'.");

            TempData["SuccessMessage"] = "Group created successfully.";
            return RedirectToAction("Groups");
        }

        [RequireRight(RightsCatalog.AdminManageGroups)]
        public IActionResult EditGroup(int id)
        {
            var group = _context.UserGroups.FirstOrDefault(g => g.Id == id);
            if (group == null)
                return NotFound();

            var model = new GroupEditViewModel
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireRight(RightsCatalog.AdminManageUsers)]
        public IActionResult DeleteSelectedUsers(List<int> userIds)
        {
            var currentUser = _userContextService.CurrentUser;
            if (!IsSuperAdminUser(currentUser))
            {
                return Forbid();
            }

            if (userIds == null || userIds.Count == 0)
            {
                TempData["ErrorMessage"] = "No users selected.";
                return RedirectToAction(nameof(Users));
            }

            var messages = new System.Collections.Generic.List<string>();
            var deleted = 0;

            foreach (var userId in userIds.Distinct())
            {
                var user = _context.AppUsers
                    .Include(u => u.Group)
                    .FirstOrDefault(u => u.Id == userId);

                if (user == null)
                {
                    continue;
                }

                if (IsProtectedSystemUser(user))
                {
                    messages.Add($"User '{user.UserName}' cannot be deleted.");
                    continue;
                }

                var mappedContactGroup = ResolveContactGroupForUserGroup(user.Group);
                if (mappedContactGroup != null)
                {
                    var hasContacts = _context.Contacts.Any(c => c.GroupId == mappedContactGroup.Id);
                    if (hasContacts)
                    {
                        messages.Add($"Delete contacts in '{mappedContactGroup.Name}' first, then delete user '{user.UserName}'.");
                        continue;
                    }
                }

                _context.AppUsers.Remove(user);
                deleted++;

                _adminHistoryService.Log(
                    actionType: "Delete",
                    entityType: "User",
                    entityId: user.Id,
                    performedBy: currentUser?.UserName ?? "Unknown",
                    details: $"Deleted user '{user.UserName}'.");
            }

            if (deleted > 0)
            {
                _context.SaveChanges();
            }

            if (messages.Count > 0)
            {
                TempData["ErrorMessage"] = string.Join(" ", messages);
            }

            if (deleted > 0)
            {
                TempData["SuccessMessage"] = $"Deleted {deleted} user(s).";
            }

            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireRight(RightsCatalog.AdminManageGroups)]
        public IActionResult DeleteSelectedGroups(List<int> groupIds)
        {
            var currentUser = _userContextService.CurrentUser;
            if (!IsSuperAdminUser(currentUser))
            {
                return Forbid();
            }

            if (groupIds == null || groupIds.Count == 0)
            {
                TempData["ErrorMessage"] = "No groups selected.";
                return RedirectToAction(nameof(Groups));
            }

            var messages = new System.Collections.Generic.List<string>();
            var deleted = 0;

            foreach (var groupId in groupIds.Distinct())
            {
                var group = _context.UserGroups
                    .Include(g => g.Users)
                    .FirstOrDefault(g => g.Id == groupId);

                if (group == null)
                {
                    continue;
                }

                if (string.Equals(group.Name, "Administrators", StringComparison.OrdinalIgnoreCase))
                {
                    messages.Add("Administrators group cannot be deleted.");
                    continue;
                }

                var mappedContactGroup = ResolveContactGroupForUserGroup(group);
                if (mappedContactGroup != null)
                {
                    var hasContacts = _context.Contacts.Any(c => c.GroupId == mappedContactGroup.Id);
                    if (hasContacts)
                    {
                        messages.Add($"Delete contacts in '{mappedContactGroup.Name}' first, then delete users, then delete group '{group.Name}'.");
                        continue;
                    }
                }

                if (group.Users.Any())
                {
                    messages.Add($"Delete users in group '{group.Name}' first, then delete the group.");
                    continue;
                }

                _context.UserGroups.Remove(group);
                deleted++;

                _adminHistoryService.Log(
                    actionType: "Delete",
                    entityType: "Group",
                    entityId: group.Id,
                    performedBy: currentUser?.UserName ?? "Unknown",
                    details: $"Deleted group '{group.Name}'.");
            }

            if (deleted > 0)
            {
                _context.SaveChanges();
            }

            if (messages.Count > 0)
            {
                TempData["ErrorMessage"] = string.Join(" ", messages);
            }

            if (deleted > 0)
            {
                TempData["SuccessMessage"] = $"Deleted {deleted} group(s).";
            }

            return RedirectToAction(nameof(Groups));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireRight(RightsCatalog.AdminManageGroups)]
        public IActionResult EditGroup(GroupEditViewModel model)
        {
            var group = _context.UserGroups.FirstOrDefault(g => g.Id == model.Id);
            if (group == null)
                return NotFound();

            if (_context.UserGroups.Any(g => g.Name == model.Name && g.Id != model.Id))
            {
                ModelState.AddModelError(nameof(GroupEditViewModel.Name), "Group name already exists.");
            }

            if (!ModelState.IsValid)
                return View(model);

            if (string.Equals(group.Name, "Administrators", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(model.Name, "Administrators", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(GroupEditViewModel.Name), "Administrators group name cannot be changed.");
                return View(model);
            }

            group.Name = model.Name;
            group.Description = model.Description;
            _context.SaveChanges();

            _adminHistoryService.Log(
                actionType: "Edit",
                entityType: "Group",
                entityId: group.Id,
                performedBy: _userContextService.CurrentUser?.UserName ?? "Unknown",
                details: $"Edited group '{group.Name}'.");

            TempData["SuccessMessage"] = "Group updated successfully.";
            return RedirectToAction("Groups");
        }

        [RequireRight(RightsCatalog.AdminManageRights)]
        public IActionResult GroupRights(int id)
        {
            var group = _context.UserGroups.FirstOrDefault(g => g.Id == id);
            if (group == null)
                return NotFound();

            var groupRights = _context.GroupRights
                .Where(r => r.UserGroupId == id)
                .ToList();

            var rights = RightsCatalog.All.Select(r =>
            {
                var existing = groupRights.FirstOrDefault(gr => gr.RightKey == r.Key);
                return new RightAssignmentViewModel
                {
                    Key = r.Key,
                    Label = r.Label,
                    Category = r.Category,
                    IsGranted = existing?.IsGranted ?? false
                };
            }).ToList();

            var model = new GroupRightsViewModel
            {
                GroupId = group.Id,
                GroupName = group.Name,
                Rights = rights
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireRight(RightsCatalog.AdminManageRights)]
        public IActionResult GroupRights(GroupRightsViewModel model)
        {
            foreach (var right in model.Rights)
            {
                var existing = _context.GroupRights.FirstOrDefault(r => r.UserGroupId == model.GroupId && r.RightKey == right.Key);
                if (existing == null)
                {
                    _context.GroupRights.Add(new GroupRight
                    {
                        UserGroupId = model.GroupId,
                        RightKey = right.Key,
                        IsGranted = right.IsGranted
                    });
                }
                else
                {
                    existing.IsGranted = right.IsGranted;
                }
            }

            _context.SaveChanges();

            _adminHistoryService.Log(
                actionType: "Edit",
                entityType: "GroupRights",
                entityId: model.GroupId,
                performedBy: _userContextService.CurrentUser?.UserName ?? "Unknown",
                details: $"Updated rights for group '{model.GroupName}'.");

            TempData["SuccessMessage"] = "Group rights updated successfully.";
            return RedirectToAction("GroupRights", new { id = model.GroupId });
        }

        [RequireRight(RightsCatalog.AdminManageUsers)]
        public IActionResult History(int take = 200)
        {
            var entries = _adminHistoryService
                .GetLatest(take)
                .Select(e => new AdminHistoryEntryViewModel
                {
                    ActionType = e.ActionType,
                    EntityType = e.EntityType,
                    EntityId = e.EntityId,
                    Details = e.Details,
                    PerformedBy = e.PerformedBy,
                    PerformedAt = e.PerformedAt
                })
                .ToList();

            var model = new AdminHistoryListViewModel
            {
                Entries = entries
            };

            return View(model);
        }

        [RequireRight(RightsCatalog.AdminManageUsers)]
        public IActionResult RestoreContacts()
        {
            var currentUser = _userContextService.CurrentUser;
            if (!IsSuperAdminUser(currentUser))
            {
                return Forbid();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireRight(RightsCatalog.AdminManageUsers)]
        public async Task<IActionResult> RestoreContacts(IFormFile backupZip)
        {
            var currentUser = _userContextService.CurrentUser;
            if (!IsSuperAdminUser(currentUser))
            {
                return Forbid();
            }

            if (backupZip == null || backupZip.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a backup ZIP file.";
                return RedirectToAction(nameof(RestoreContacts));
            }

            var tempRoot = Path.Combine(Path.GetTempPath(), "cms-restore-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempRoot);

            try
            {
                var zipPath = Path.Combine(tempRoot, "backup.zip");
                await using (var fs = System.IO.File.Create(zipPath))
                {
                    await backupZip.CopyToAsync(fs);
                }

                ZipFile.ExtractToDirectory(zipPath, tempRoot);

                // Expect: ContactManagement.db + uploads/photos + uploads/documents
                var backupDbPath = Directory.GetFiles(tempRoot, "ContactManagement.db", SearchOption.AllDirectories)
                    .FirstOrDefault();

                if (string.IsNullOrWhiteSpace(backupDbPath) || !System.IO.File.Exists(backupDbPath))
                {
                    TempData["ErrorMessage"] = "Backup ZIP must contain ContactManagement.db";
                    return RedirectToAction(nameof(RestoreContacts));
                }

                var backupUploadsRoot = Directory.GetDirectories(tempRoot, "uploads", SearchOption.AllDirectories)
                    .FirstOrDefault();

                if (string.IsNullOrWhiteSpace(backupUploadsRoot) || !Directory.Exists(backupUploadsRoot))
                {
                    TempData["ErrorMessage"] = "Backup ZIP must contain uploads/photos and uploads/documents.";
                    return RedirectToAction(nameof(RestoreContacts));
                }

                var targetUploadsRoot = GetUploadsRoot();

                var backupConnString = new SqliteConnectionStringBuilder { DataSource = backupDbPath }.ToString();
                await using var backupConn = new SqliteConnection(backupConnString);
                await backupConn.OpenAsync();

                var wantedNames = new[] { "ABRAHAM", "PREMA", "PONNURAJ" };
                var contactsToRestore = new System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, object?>>();

                var contactCmd = backupConn.CreateCommand();
                contactCmd.CommandText = @"
SELECT Id, FirstName, LastName, NickName, Gender, DateOfBirth, Email,
       Mobile1, Mobile2, Mobile3, WhatsAppNumber,
       PassportNumber, PanNumber, AadharNumber, DrivingLicenseNumber, VotersId,
       BankAccountNumber, BankName, BranchName, IfscCode,
       Address, City, State, PostalCode, Country,
       PhotoPath, GroupId, OtherDetails, CreatedAt, UpdatedAt
FROM Contacts
WHERE lower(FirstName) IN ('abraham','prema','ponnuraj');";

                await using (var reader = await contactCmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var dict = new System.Collections.Generic.Dictionary<string, object?>();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            dict[reader.GetName(i)] = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
                        }
                        contactsToRestore.Add(dict);
                    }
                }

                if (contactsToRestore.Count == 0)
                {
                    TempData["ErrorMessage"] = "No Abraham/Prema/Ponnuraj contacts were found in the backup DB.";
                    return RedirectToAction(nameof(RestoreContacts));
                }

                // Pull related rows from backup
                var backupContactIds = contactsToRestore.Select(c => Convert.ToInt32(c["Id"])).ToArray();
                var idList = string.Join(",", backupContactIds);

                var backupPhotos = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, object?>>>();
                var photoCmd = backupConn.CreateCommand();
                photoCmd.CommandText = $@"
SELECT ContactId, PhotoPath, FileName, FileSize, ContentType, IsProfilePhoto, UploadedAt
FROM ContactPhotos
WHERE ContactId IN ({idList});";
                await using (var reader = await photoCmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var contactId = reader.GetInt32(0);
                        if (!backupPhotos.TryGetValue(contactId, out var list))
                        {
                            list = new System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, object?>>();
                            backupPhotos[contactId] = list;
                        }

                        list.Add(new System.Collections.Generic.Dictionary<string, object?>
                        {
                            ["PhotoPath"] = reader.IsDBNull(1) ? null : reader.GetString(1),
                            ["FileName"] = reader.IsDBNull(2) ? null : reader.GetString(2),
                            ["FileSize"] = reader.IsDBNull(3) ? 0L : reader.GetInt64(3),
                            ["ContentType"] = reader.IsDBNull(4) ? null : reader.GetString(4),
                            ["IsProfilePhoto"] = !reader.IsDBNull(5) && reader.GetBoolean(5),
                            ["UploadedAt"] = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6)
                        });
                    }
                }

                var backupDocs = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, object?>>>();
                var docCmd = backupConn.CreateCommand();
                docCmd.CommandText = $@"
SELECT ContactId, DocumentPath, FileName, FileSize, ContentType, DocumentType, UploadedAt
FROM ContactDocuments
WHERE ContactId IN ({idList});";
                await using (var reader = await docCmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var contactId = reader.GetInt32(0);
                        if (!backupDocs.TryGetValue(contactId, out var list))
                        {
                            list = new System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, object?>>();
                            backupDocs[contactId] = list;
                        }

                        list.Add(new System.Collections.Generic.Dictionary<string, object?>
                        {
                            ["DocumentPath"] = reader.IsDBNull(1) ? null : reader.GetString(1),
                            ["FileName"] = reader.IsDBNull(2) ? null : reader.GetString(2),
                            ["FileSize"] = reader.IsDBNull(3) ? 0L : reader.GetInt64(3),
                            ["ContentType"] = reader.IsDBNull(4) ? null : reader.GetString(4),
                            ["DocumentType"] = reader.IsDBNull(5) ? null : reader.GetString(5),
                            ["UploadedAt"] = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6)
                        });
                    }
                }

                var backupBanks = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, object?>>>();
                var bankCmd = backupConn.CreateCommand();
                bankCmd.CommandText = $@"
SELECT ContactId, AccountNumber, BankName, BranchName, IfscCode, CreatedAt, UpdatedAt
FROM ContactBankAccounts
WHERE ContactId IN ({idList});";
                await using (var reader = await bankCmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var contactId = reader.GetInt32(0);
                        if (!backupBanks.TryGetValue(contactId, out var list))
                        {
                            list = new System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, object?>>();
                            backupBanks[contactId] = list;
                        }

                        list.Add(new System.Collections.Generic.Dictionary<string, object?>
                        {
                            ["AccountNumber"] = reader.IsDBNull(1) ? null : reader.GetString(1),
                            ["BankName"] = reader.IsDBNull(2) ? null : reader.GetString(2),
                            ["BranchName"] = reader.IsDBNull(3) ? null : reader.GetString(3),
                            ["IfscCode"] = reader.IsDBNull(4) ? null : reader.GetString(4),
                            ["CreatedAt"] = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                            ["UpdatedAt"] = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6)
                        });
                    }
                }

                // Merge into current DB
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;

                var restoredCount = 0;
                var copiedFiles = 0;

                foreach (var backupContact in contactsToRestore)
                {
                    var firstName = backupContact["FirstName"]?.ToString() ?? string.Empty;
                    var lastName = backupContact["LastName"]?.ToString();
                    var nickName = backupContact["NickName"]?.ToString();

                    var normalizedFirstName = NormalizeText(firstName);
                    if (!wantedNames.Contains(normalizedFirstName))
                    {
                        continue;
                    }

                    var normalizedLastName = NormalizeText(lastName);
                    var normalizedNickName = NormalizeText(nickName);

                    var existing = _context.Contacts
                        .Include(c => c.Photos)
                        .Include(c => c.Documents)
                        .Include(c => c.BankAccounts)
                        .FirstOrDefault(c =>
                            NormalizeText(c.FirstName) == normalizedFirstName &&
                            NormalizeText(c.LastName) == normalizedLastName &&
                            NormalizeText(c.NickName) == normalizedNickName);

                    if (existing == null)
                    {
                        existing = new Contact();
                        _context.Contacts.Add(existing);
                    }

                    // Copy scalar fields
                    existing.FirstName = firstName;
                    existing.LastName = lastName;
                    existing.NickName = nickName;
                    existing.Gender = backupContact["Gender"]?.ToString();
                    existing.DateOfBirth = backupContact["DateOfBirth"] as DateTime?;
                    existing.Email = backupContact["Email"]?.ToString();
                    existing.Mobile1 = backupContact["Mobile1"]?.ToString();
                    existing.Mobile2 = backupContact["Mobile2"]?.ToString();
                    existing.Mobile3 = backupContact["Mobile3"]?.ToString();
                    existing.WhatsAppNumber = backupContact["WhatsAppNumber"]?.ToString();
                    existing.PassportNumber = backupContact["PassportNumber"]?.ToString();
                    existing.PanNumber = backupContact["PanNumber"]?.ToString();
                    existing.AadharNumber = backupContact["AadharNumber"]?.ToString();
                    existing.DrivingLicenseNumber = backupContact["DrivingLicenseNumber"]?.ToString();
                    existing.VotersId = backupContact["VotersId"]?.ToString();
                    existing.BankAccountNumber = backupContact["BankAccountNumber"]?.ToString();
                    existing.BankName = backupContact["BankName"]?.ToString();
                    existing.BranchName = backupContact["BranchName"]?.ToString();
                    existing.IfscCode = backupContact["IfscCode"]?.ToString();
                    existing.Address = backupContact["Address"]?.ToString();
                    existing.City = backupContact["City"]?.ToString();
                    existing.State = backupContact["State"]?.ToString();
                    existing.PostalCode = backupContact["PostalCode"]?.ToString();
                    existing.Country = backupContact["Country"]?.ToString();
                    existing.PhotoPath = backupContact["PhotoPath"]?.ToString();
                    existing.GroupId = backupContact["GroupId"] as int?;
                    existing.OtherDetails = backupContact["OtherDetails"]?.ToString();

                    existing.CreatedAt = backupContact["CreatedAt"] as DateTime? ?? existing.CreatedAt;
                    existing.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();

                    // Replace related data
                    if (existing.Photos.Any())
                    {
                        _context.ContactPhotos.RemoveRange(existing.Photos);
                    }
                    if (existing.Documents.Any())
                    {
                        _context.ContactDocuments.RemoveRange(existing.Documents);
                    }
                    if (existing.BankAccounts.Any())
                    {
                        _context.ContactBankAccounts.RemoveRange(existing.BankAccounts);
                    }
                    await _context.SaveChangesAsync();

                    var sourceId = Convert.ToInt32(backupContact["Id"]);

                    if (backupPhotos.TryGetValue(sourceId, out var photos))
                    {
                        foreach (var row in photos)
                        {
                            var photoPath = row["PhotoPath"]?.ToString();
                            if (!string.IsNullOrWhiteSpace(photoPath))
                            {
                                CopyBackupFile(backupUploadsRoot, targetUploadsRoot, photoPath, ref copiedFiles);
                            }

                            _context.ContactPhotos.Add(new ContactPhoto
                            {
                                ContactId = existing.Id,
                                PhotoPath = photoPath ?? string.Empty,
                                FileName = row["FileName"]?.ToString() ?? string.Empty,
                                FileSize = row["FileSize"] is long l ? l : 0L,
                                ContentType = row["ContentType"]?.ToString() ?? "application/octet-stream",
                                IsProfilePhoto = row["IsProfilePhoto"] is bool b && b,
                                UploadedAt = row["UploadedAt"] as DateTime? ?? DateTime.Now
                            });
                        }
                    }

                    if (backupDocs.TryGetValue(sourceId, out var docs))
                    {
                        foreach (var row in docs)
                        {
                            var docPath = row["DocumentPath"]?.ToString();
                            if (!string.IsNullOrWhiteSpace(docPath))
                            {
                                CopyBackupFile(backupUploadsRoot, targetUploadsRoot, docPath, ref copiedFiles);
                            }

                            _context.ContactDocuments.Add(new ContactDocument
                            {
                                ContactId = existing.Id,
                                DocumentPath = docPath ?? string.Empty,
                                FileName = row["FileName"]?.ToString() ?? string.Empty,
                                FileSize = row["FileSize"] is long l ? l : 0L,
                                ContentType = row["ContentType"]?.ToString() ?? "application/octet-stream",
                                DocumentType = row["DocumentType"]?.ToString() ?? "Other",
                                UploadedAt = row["UploadedAt"] as DateTime? ?? DateTime.Now
                            });
                        }
                    }

                    if (backupBanks.TryGetValue(sourceId, out var banks))
                    {
                        foreach (var row in banks)
                        {
                            _context.ContactBankAccounts.Add(new ContactBankAccount
                            {
                                ContactId = existing.Id,
                                AccountNumber = row["AccountNumber"]?.ToString(),
                                BankName = row["BankName"]?.ToString(),
                                BranchName = row["BranchName"]?.ToString(),
                                IfscCode = row["IfscCode"]?.ToString(),
                                CreatedAt = row["CreatedAt"] as DateTime? ?? DateTime.Now,
                                UpdatedAt = row["UpdatedAt"] as DateTime? ?? DateTime.Now
                            });
                        }
                    }

                    await _context.SaveChangesAsync();
                    restoredCount++;
                }

                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                _adminHistoryService.Log(
                    actionType: "Restore",
                    entityType: "Contacts",
                    entityId: 0,
                    performedBy: currentUser?.UserName ?? "Unknown",
                    details: $"Restored {restoredCount} key contacts from backup (copied {copiedFiles} files)."
                );

                TempData["SuccessMessage"] = $"Restored {restoredCount} contact(s) and copied {copiedFiles} file(s).";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Restore failed: {ex.Message}";
                return RedirectToAction(nameof(RestoreContacts));
            }
            finally
            {
                try
                {
                    if (Directory.Exists(tempRoot))
                    {
                        Directory.Delete(tempRoot, recursive: true);
                    }
                }
                catch
                {
                    // ignore cleanup issues
                }
            }

            static void CopyBackupFile(string backupUploadsRoot, string targetUploadsRoot, string webPath, ref int copiedFiles)
            {
                var normalized = webPath.Trim();
                if (normalized.StartsWith("/") == false)
                {
                    normalized = "/" + normalized;
                }

                if (!normalized.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                var relative = normalized.Substring("/uploads/".Length).Replace('/', Path.DirectorySeparatorChar);
                var source = Path.Combine(backupUploadsRoot, relative);
                var dest = Path.Combine(targetUploadsRoot, relative);

                var destDir = Path.GetDirectoryName(dest);
                if (!string.IsNullOrWhiteSpace(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }

                if (System.IO.File.Exists(source))
                {
                    System.IO.File.Copy(source, dest, overwrite: true);
                    copiedFiles++;
                }
            }
        }
    }
}
