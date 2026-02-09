using System;
using System.Linq;
using ContactManagementAPI.Data;
using ContactManagementAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace ContactManagementAPI.Services
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            if (!context.UserGroups.Any())
            {
                var adminGroup = new UserGroup
                {
                    Name = "Administrators",
                    Description = "System administrators with full access",
                    CreatedAt = DateTime.Now
                };

                context.UserGroups.Add(adminGroup);
                context.SaveChanges();

                var groupRights = RightsCatalog.All
                    .Select(r => new GroupRight
                    {
                        UserGroupId = adminGroup.Id,
                        RightKey = r.Key,
                        IsGranted = true
                    })
                    .ToList();

                context.GroupRights.AddRange(groupRights);
                context.SaveChanges();
            }

            if (!context.AppUsers.Any())
            {
                var adminGroupId = context.UserGroups
                    .Where(g => g.Name == "Administrators")
                    .Select(g => g.Id)
                    .First();

                var adminUser = new AppUser
                {
                    UserName = "admin",
                    FullName = "System Administrator",
                    IsAdmin = true,
                    IsActive = true,
                    GroupId = adminGroupId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                var hasher = new PasswordHasher<AppUser>();
                adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin@123");

                context.AppUsers.Add(adminUser);
                context.SaveChanges();
            }
        }
    }
}
