using System.Linq;
using ContactManagementAPI.Data;
using ContactManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ContactManagementAPI.Services
{
    public class AuthorizationService
    {
        private readonly ApplicationDbContext _context;

        private static bool IsSuperAdminUser(AppUser? user)
        {
            return user != null && string.Equals(user.UserName, SeedData.SuperAdminUserName, System.StringComparison.OrdinalIgnoreCase);
        }

        public AuthorizationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool HasRight(int userId, string rightKey)
        {
            var user = _context.AppUsers
                .AsNoTracking()
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return false;

            if (IsSuperAdminUser(user))
                return true;

            if (!user.IsActive)
                return false;

            if (user.IsAdmin)
                return true;

            var userRight = _context.UserRights
                .AsNoTracking()
                .FirstOrDefault(r => r.AppUserId == userId && r.RightKey == rightKey);

            if (userRight != null)
                return userRight.IsGranted;

            var groupRight = _context.GroupRights
                .AsNoTracking()
                .FirstOrDefault(r => r.UserGroupId == user.GroupId && r.RightKey == rightKey);

            return groupRight?.IsGranted ?? false;
        }

        public bool IsAdmin(int userId)
        {
            var user = _context.AppUsers
                .AsNoTracking()
                .FirstOrDefault(u => u.Id == userId);

            if (IsSuperAdminUser(user))
                return true;

            return user?.IsAdmin == true && user.IsActive;
        }
    }
}
