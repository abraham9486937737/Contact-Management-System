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
        public const string SuperAdminUserName = "abrahamcbe@gmail.com";
        public const string SuperAdminDefaultPassword = "M@ld1ves";

        private static string GetSuperAdminPassword()
        {
            var fromEnv = Environment.GetEnvironmentVariable("SUPERADMIN_PASSWORD");
            return string.IsNullOrWhiteSpace(fromEnv) ? SuperAdminDefaultPassword : fromEnv;
        }

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

            EnsureSuperAdminUser(context, hasher);

            EnsureContactGroupUsers(context, hasher);

            EnsureInitialContacts(context);
        }

        private static void EnsureInitialContacts(ApplicationDbContext context)
        {
            if (context.Contacts.Any())
            {
                return;
            }

            var now = DateTime.Now;

            var familyGroupId = context.ContactGroups
                .Where(g => g.Name == "Family")
                .Select(g => (int?)g.Id)
                .FirstOrDefault();

            var seedContacts = new[]
            {
                new Contact
                {
                    FirstName = "Abraham",
                    LastName = "CBE",
                    NickName = "Abraham",
                    Mobile1 = "9000000001",
                    GroupId = familyGroupId,
                    CreatedAt = now,
                    UpdatedAt = now,
                    OtherDetails = "Seeded default contact (restored after fresh deployment)."
                },
                new Contact
                {
                    FirstName = "Prema",
                    LastName = "",
                    NickName = "Prema",
                    Mobile1 = "9000000002",
                    GroupId = familyGroupId,
                    CreatedAt = now,
                    UpdatedAt = now,
                    OtherDetails = "Seeded default contact (restored after fresh deployment)."
                },
                new Contact
                {
                    FirstName = "Ponnuraj",
                    LastName = "",
                    NickName = "Ponnuraj",
                    Mobile1 = "9000000003",
                    GroupId = familyGroupId,
                    CreatedAt = now,
                    UpdatedAt = now,
                    OtherDetails = "Seeded default contact (restored after fresh deployment)."
                }
            };

            context.Contacts.AddRange(seedContacts);
            context.SaveChanges();

            // Add one test contact per contact group (to help validate group scoping after a fresh DB)
            var contactGroups = context.ContactGroups
                .OrderBy(g => g.Id)
                .ToList();

            var counter = 10;
            foreach (var group in contactGroups)
            {
                var testContact = new Contact
                {
                    FirstName = $"{group.Name} Test",
                    LastName = "Contact",
                    NickName = $"{group.Name}",
                    Mobile1 = $"90000000{counter:D2}",
                    GroupId = group.Id,
                    CreatedAt = now,
                    UpdatedAt = now,
                    OtherDetails = "Seeded test contact for group validation."
                };

                context.Contacts.Add(testContact);
                counter++;
            }

            context.SaveChanges();
        }

        private static void EnsureSuperAdminUser(ApplicationDbContext context, PasswordHasher<AppUser> hasher)
        {
            var adminGroupId = context.UserGroups
                .Where(g => g.Name == "Administrators")
                .Select(g => g.Id)
                .FirstOrDefault();

            if (adminGroupId <= 0)
            {
                return;
            }

            var existing = context.AppUsers.FirstOrDefault(u => u.UserName == SuperAdminUserName);
            if (existing == null)
            {
                var password = GetSuperAdminPassword();
                var user = new AppUser
                {
                    UserName = SuperAdminUserName,
                    FullName = "Abraham",
                    IsAdmin = true,
                    IsActive = true,
                    GroupId = adminGroupId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                user.PasswordHash = hasher.HashPassword(user, password);
                context.AppUsers.Add(user);
                context.SaveChanges();
                return;
            }

            var updated = false;
            var enforcedPassword = GetSuperAdminPassword();

            if (!existing.IsAdmin)
            {
                existing.IsAdmin = true;
                updated = true;
            }

            if (!existing.IsActive)
            {
                existing.IsActive = true;
                updated = true;
            }

            if (existing.GroupId != adminGroupId)
            {
                existing.GroupId = adminGroupId;
                updated = true;
            }

            if (string.IsNullOrWhiteSpace(existing.FullName))
            {
                existing.FullName = "Abraham";
                updated = true;
            }

            // Enforce Super Admin password so it matches the required credential.
            existing.PasswordHash = hasher.HashPassword(existing, enforcedPassword);
            updated = true;

            if (updated)
            {
                existing.UpdatedAt = DateTime.Now;
                context.SaveChanges();
            }
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
