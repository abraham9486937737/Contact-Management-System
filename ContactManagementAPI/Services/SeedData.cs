using System;
using System.Linq;
using System.Text.RegularExpressions;
using ContactManagementAPI.Data;
using ContactManagementAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace ContactManagementAPI.Services
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            var hasher = new PasswordHasher<AppUser>();

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
            else
            {
                var adminGroup = context.UserGroups.FirstOrDefault(g => g.Name == "Administrators");
                if (adminGroup != null)
                {
                    var existingAdminRights = context.GroupRights
                        .Where(r => r.UserGroupId == adminGroup.Id)
                        .ToList();

                    foreach (var right in RightsCatalog.All)
                    {
                        var existing = existingAdminRights.FirstOrDefault(r => r.RightKey == right.Key);
                        if (existing == null)
                        {
                            context.GroupRights.Add(new GroupRight
                            {
                                UserGroupId = adminGroup.Id,
                                RightKey = right.Key,
                                IsGranted = true
                            });
                        }
                        else if (!existing.IsGranted)
                        {
                            existing.IsGranted = true;
                        }
                    }

                    context.SaveChanges();
                }
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

                adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin@123");

                context.AppUsers.Add(adminUser);
                context.SaveChanges();
            }

            EnsureContactGroupUsers(context, hasher);
        }

        private static void EnsureContactGroupUsers(ApplicationDbContext context, PasswordHasher<AppUser> hasher)
        {
            var contactGroups = context.ContactGroups
                .OrderBy(g => g.Name)
                .ToList();

            foreach (var contactGroup in contactGroups)
            {
                var userGroupName = $"ContactGroup - {contactGroup.Name}";
                var userGroup = context.UserGroups.FirstOrDefault(g => g.Name == userGroupName);
                if (userGroup == null)
                {
                    userGroup = new UserGroup
                    {
                        Name = userGroupName,
                        Description = $"Full access group for contact group '{contactGroup.Name}'",
                        CreatedAt = DateTime.Now
                    };

                    context.UserGroups.Add(userGroup);
                    context.SaveChanges();
                }

                var groupRights = context.GroupRights
                    .Where(r => r.UserGroupId == userGroup.Id)
                    .ToList();

                foreach (var right in RightsCatalog.All)
                {
                    var existing = groupRights.FirstOrDefault(r => r.RightKey == right.Key);
                    if (existing == null)
                    {
                        context.GroupRights.Add(new GroupRight
                        {
                            UserGroupId = userGroup.Id,
                            RightKey = right.Key,
                            IsGranted = true
                        });
                    }
                    else if (!existing.IsGranted)
                    {
                        existing.IsGranted = true;
                    }
                }

                context.SaveChanges();

                var preferredUserName = BuildGroupUserName(contactGroup.Name);
                var appUser = context.AppUsers.FirstOrDefault(u => u.UserName == preferredUserName);
                if (appUser == null)
                {
                    appUser = new AppUser
                    {
                        UserName = preferredUserName,
                        FullName = $"{contactGroup.Name} Group User",
                        GroupId = userGroup.Id,
                        IsAdmin = false,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    appUser.PasswordHash = hasher.HashPassword(appUser, "Group@123");
                    context.AppUsers.Add(appUser);
                }
                else
                {
                    appUser.GroupId = userGroup.Id;
                    appUser.IsActive = true;
                    appUser.UpdatedAt = DateTime.Now;
                }

                context.SaveChanges();
            }
        }

        private static string BuildGroupUserName(string? groupName)
        {
            var raw = string.IsNullOrWhiteSpace(groupName) ? "group" : groupName.Trim().ToLowerInvariant();
            var slug = Regex.Replace(raw, "[^a-z0-9]+", ".").Trim('.');
            if (string.IsNullOrWhiteSpace(slug))
            {
                slug = "group";
            }

            return $"{slug}.user";
        }
    }
}
