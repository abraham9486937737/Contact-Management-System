using System;
using System.Linq;
using ContactManagementAPI.Data;
using ContactManagementAPI.Models;
using ContactManagementAPI.Security;
using ContactManagementAPI.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContactManagementAPI.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<AppUser> _passwordHasher = new();

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [RequireRight(RightsCatalog.AdminManageUsers)]
        public IActionResult Users()
        {
            var users = _context.AppUsers
                .Include(u => u.Group)
                .OrderBy(u => u.UserName)
                .ToList();

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

            TempData["SuccessMessage"] = "User created successfully.";
            return RedirectToAction("Users");
        }

        [RequireRight(RightsCatalog.AdminManageUsers)]
        public IActionResult EditUser(int id)
        {
            var user = _context.AppUsers.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound();

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
            var user = _context.AppUsers.FirstOrDefault(u => u.Id == model.Id);
            if (user == null)
                return NotFound();

            if (_context.AppUsers.Any(u => u.UserName == model.UserName && u.Id != model.Id))
            {
                ModelState.AddModelError(nameof(UserEditViewModel.UserName), "User name already exists.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["Groups"] = _context.UserGroups.OrderBy(g => g.Name).ToList();
                return View(model);
            }

            user.UserName = model.UserName;
            user.FullName = model.FullName;
            user.GroupId = model.GroupId;
            user.IsAdmin = model.IsAdmin;
            user.IsActive = model.IsActive;
            user.UpdatedAt = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
            }

            _context.SaveChanges();
            TempData["SuccessMessage"] = "User updated successfully.";
            return RedirectToAction("Users");
        }

        [RequireRight(RightsCatalog.AdminManageRights)]
        public IActionResult UserRights(int id)
        {
            var user = _context.AppUsers
                .Include(u => u.Group)
                .FirstOrDefault(u => u.Id == id);

            if (user == null)
                return NotFound();

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
            var user = _context.AppUsers.FirstOrDefault(u => u.Id == model.UserId);
            if (user == null)
                return NotFound();

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
            TempData["SuccessMessage"] = "User rights updated successfully.";
            return RedirectToAction("UserRights", new { id = model.UserId });
        }

        [RequireRight(RightsCatalog.AdminManageGroups)]
        public IActionResult Groups()
        {
            var groups = _context.UserGroups
                .OrderBy(g => g.Name)
                .ToList();

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

            group.Name = model.Name;
            group.Description = model.Description;
            _context.SaveChanges();

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
            TempData["SuccessMessage"] = "Group rights updated successfully.";
            return RedirectToAction("GroupRights", new { id = model.GroupId });
        }
    }
}
