using ContactManagementAPI.Data;
using ContactManagementAPI.Models;
using ContactManagementAPI.Services;
using ContactManagementAPI.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContactManagementAPI.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<AppUser> _passwordHasher = new();

        private static string NormalizeUserName(string? userName)
        {
            return (userName ?? string.Empty).Trim().ToUpperInvariant();
        }

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var normalizedUserName = NormalizeUserName(model.UserName);
            var normalizedPassword = (model.Password ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(normalizedUserName) || string.IsNullOrWhiteSpace(normalizedPassword))
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }

            var user = _context.AppUsers
                .Include(u => u.Group)
                .FirstOrDefault(u => u.UserName.ToUpper() == normalizedUserName);

            if (user == null || !user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, normalizedPassword);
            if (result == PasswordVerificationResult.Failed)
            {
                var isLegacyPlainTextPassword = !string.IsNullOrWhiteSpace(user.PasswordHash) &&
                                                !user.PasswordHash.StartsWith("AQAAAA", StringComparison.Ordinal);

                if (!isLegacyPlainTextPassword || !string.Equals(user.PasswordHash.Trim(), normalizedPassword, StringComparison.Ordinal))
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                    return View(model);
                }

                user.PasswordHash = _passwordHasher.HashPassword(user, normalizedPassword);
                user.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
            }

            HttpContext.Session.SetInt32(SessionKeys.UserId, user.Id);
            return Redirect(string.IsNullOrWhiteSpace(model.ReturnUrl) ? "/" : model.ReturnUrl);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
