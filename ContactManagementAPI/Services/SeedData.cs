using System;
using System.Collections.Generic;
using System.IO;
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

            EnsurePinnedContactsWithDetails(context);

            EnsureDetailedTestContactsForAllGroups(context);
        }

        private static void EnsurePinnedContactsWithDetails(ApplicationDbContext context)
        {
            var now = DateTime.Now;
            var familyGroupId = context.ContactGroups.Where(g => g.Name == "Family").Select(g => (int?)g.Id).FirstOrDefault();
            var friendsGroupId = context.ContactGroups.Where(g => g.Name == "Friends").Select(g => (int?)g.Id).FirstOrDefault();

            var pinnedContacts = new List<SeedContactSpec>
            {
                new SeedContactSpec
                {
                    FirstName = "Abraham",
                    LastName = "CBE",
                    NickName = "Abraham",
                    Email = "abrahamcbe@gmail.com",
                    Mobile1 = "+919486937737",
                    WhatsAppNumber = "+919486937737",
                    Gender = "Male",
                    DateOfBirth = new DateTime(1988, 7, 15),
                    Address = "RS Puram",
                    City = "Coimbatore",
                    State = "Tamil Nadu",
                    PostalCode = "641002",
                    Country = "India",
                    PassportNumber = "P1234567",
                    PanNumber = "ABCDE1234F",
                    AadharNumber = "1234-5678-9012",
                    DrivingLicenseNumber = "TN37-2020-0001111",
                    VotersId = "TNV1234567",
                    GroupId = familyGroupId,
                    OtherDetails = "Primary contact seeded with full details.",
                    MediaTag = "abraham.cbe"
                },
                new SeedContactSpec
                {
                    FirstName = "Prema",
                    LastName = "CBE",
                    NickName = "Prema",
                    Email = "prema.cbe@example.com",
                    Mobile1 = "+919876543210",
                    WhatsAppNumber = "+919876543210",
                    Gender = "Female",
                    DateOfBirth = new DateTime(1990, 2, 10),
                    Address = "Saibaba Colony",
                    City = "Coimbatore",
                    State = "Tamil Nadu",
                    PostalCode = "641011",
                    Country = "India",
                    PassportNumber = "P2345678",
                    PanNumber = "BCDEA2345G",
                    AadharNumber = "2345-6789-0123",
                    DrivingLicenseNumber = "TN37-2020-0002222",
                    VotersId = "TNV2345678",
                    GroupId = familyGroupId,
                    OtherDetails = "Primary contact seeded with full details.",
                    MediaTag = "prema.cbe"
                },
                new SeedContactSpec
                {
                    FirstName = "Ponnuraj",
                    LastName = "CBE",
                    NickName = "Ponnuraj",
                    Email = "ponnuraj.cbe@example.com",
                    Mobile1 = "+919765432109",
                    WhatsAppNumber = "+919765432109",
                    Gender = "Male",
                    DateOfBirth = new DateTime(1985, 12, 5),
                    Address = "Gandhipuram",
                    City = "Coimbatore",
                    State = "Tamil Nadu",
                    PostalCode = "641012",
                    Country = "India",
                    PassportNumber = "P3456789",
                    PanNumber = "CDEAB3456H",
                    AadharNumber = "3456-7890-1234",
                    DrivingLicenseNumber = "TN37-2020-0003333",
                    VotersId = "TNV3456789",
                    GroupId = friendsGroupId ?? familyGroupId,
                    OtherDetails = "Primary contact seeded with full details.",
                    MediaTag = "ponnuraj.cbe"
                }
            };

            foreach (var spec in pinnedContacts)
            {
                var contact = UpsertContact(context, spec, now);
                EnsureContactMediaAndAttachments(context, contact, spec.MediaTag, now);
            }
        }

        private static void EnsureDetailedTestContactsForAllGroups(ApplicationDbContext context)
        {
            var now = DateTime.Now;
            var groups = context.ContactGroups.OrderBy(g => g.Id).ToList();

            foreach (var group in groups)
            {
                var groupSlug = BuildGroupUserName(group.Name).Replace(".user", string.Empty);
                var spec = new SeedContactSpec
                {
                    FirstName = group.Name,
                    LastName = "Test Contact",
                    NickName = $"{group.Name} Test",
                    Email = $"{groupSlug}.test@example.com",
                    Mobile1 = BuildGroupMobile(group.Id),
                    WhatsAppNumber = BuildGroupMobile(group.Id),
                    Gender = "Not Specified",
                    DateOfBirth = new DateTime(1995, 1, 1).AddDays(group.Id),
                    Address = $"{group.Name} Street",
                    City = "Coimbatore",
                    State = "Tamil Nadu",
                    PostalCode = $"6410{group.Id:D2}",
                    Country = "India",
                    PassportNumber = $"TP{group.Id:D6}",
                    PanNumber = $"TEST{group.Id:D4}P",
                    AadharNumber = $"9{group.Id:D3}-7{group.Id:D3}-5{group.Id:D3}",
                    DrivingLicenseNumber = $"TN37-TEST-{group.Id:D7}",
                    VotersId = $"TNV{group.Id:D7}",
                    GroupId = group.Id,
                    OtherDetails = "Detailed test contact for group validation.",
                    MediaTag = $"group.{groupSlug}.test"
                };

                var contact = UpsertContact(context, spec, now);
                EnsureContactMediaAndAttachments(context, contact, spec.MediaTag, now);
            }
        }

        private static Contact UpsertContact(ApplicationDbContext context, SeedContactSpec spec, DateTime now)
        {
            var firstName = spec.FirstName?.Trim() ?? string.Empty;
            var lastName = spec.LastName?.Trim();

            var existing = context.Contacts.FirstOrDefault(c =>
                c.FirstName.ToLower() == firstName.ToLower() &&
                (c.LastName ?? string.Empty).ToLower() == (lastName ?? string.Empty).ToLower());

            if (existing == null)
            {
                existing = new Contact
                {
                    FirstName = firstName,
                    LastName = lastName,
                    CreatedAt = now,
                    UpdatedAt = now
                };
                context.Contacts.Add(existing);
            }

            existing.NickName = UseIfMissing(existing.NickName, spec.NickName);
            existing.Gender = UseIfMissing(existing.Gender, spec.Gender);
            if (!existing.DateOfBirth.HasValue && spec.DateOfBirth.HasValue)
            {
                existing.DateOfBirth = spec.DateOfBirth.Value;
            }

            existing.Email = UseIfMissing(existing.Email, spec.Email);
            existing.Mobile1 = UseIfMissing(existing.Mobile1, spec.Mobile1);
            existing.Mobile2 = UseIfMissing(existing.Mobile2, spec.Mobile2);
            existing.Mobile3 = UseIfMissing(existing.Mobile3, spec.Mobile3);
            existing.WhatsAppNumber = UseIfMissing(existing.WhatsAppNumber, spec.WhatsAppNumber);
            existing.PassportNumber = UseIfMissing(existing.PassportNumber, spec.PassportNumber);
            existing.PanNumber = UseIfMissing(existing.PanNumber, spec.PanNumber);
            existing.AadharNumber = UseIfMissing(existing.AadharNumber, spec.AadharNumber);
            existing.DrivingLicenseNumber = UseIfMissing(existing.DrivingLicenseNumber, spec.DrivingLicenseNumber);
            existing.VotersId = UseIfMissing(existing.VotersId, spec.VotersId);
            existing.Address = UseIfMissing(existing.Address, spec.Address);
            existing.City = UseIfMissing(existing.City, spec.City);
            existing.State = UseIfMissing(existing.State, spec.State);
            existing.PostalCode = UseIfMissing(existing.PostalCode, spec.PostalCode);
            existing.Country = UseIfMissing(existing.Country, spec.Country);

            if (!existing.GroupId.HasValue && spec.GroupId.HasValue)
            {
                existing.GroupId = spec.GroupId.Value;
            }

            if (string.IsNullOrWhiteSpace(existing.OtherDetails))
            {
                existing.OtherDetails = spec.OtherDetails;
            }

            existing.UpdatedAt = now;
            context.SaveChanges();

            return existing;
        }

        private static void EnsureContactMediaAndAttachments(ApplicationDbContext context, Contact contact, string mediaTag, DateTime now)
        {
            var uploadsRoot = GetUploadsRoot();
            var photosRoot = Path.Combine(uploadsRoot, "photos");
            var documentsRoot = Path.Combine(uploadsRoot, "documents");

            Directory.CreateDirectory(photosRoot);
            Directory.CreateDirectory(documentsRoot);

            var profileFileName = $"seed_{mediaTag}_profile.png";
            var album1FileName = $"seed_{mediaTag}_album_1.png";
            var album2FileName = $"seed_{mediaTag}_album_2.png";

            var profileVirtualPath = EnsureSeedImageFile(photosRoot, profileFileName);
            var album1VirtualPath = EnsureSeedImageFile(photosRoot, album1FileName);
            var album2VirtualPath = EnsureSeedImageFile(photosRoot, album2FileName);

            if (!string.IsNullOrWhiteSpace(profileVirtualPath) && string.IsNullOrWhiteSpace(contact.PhotoPath))
            {
                contact.PhotoPath = profileVirtualPath;
                contact.UpdatedAt = now;
                context.SaveChanges();
            }

            EnsurePhotoRecord(context, contact.Id, profileVirtualPath, profileFileName, true, now);
            EnsurePhotoRecord(context, contact.Id, album1VirtualPath, album1FileName, false, now);
            EnsurePhotoRecord(context, contact.Id, album2VirtualPath, album2FileName, false, now);

            var document1Name = $"seed_{mediaTag}_id.txt";
            var document2Name = $"seed_{mediaTag}_notes.txt";

            var document1Path = EnsureSeedDocumentFile(documentsRoot, document1Name, "Seeded ID attachment generated by system setup.");
            var document2Path = EnsureSeedDocumentFile(documentsRoot, document2Name, "Seeded notes attachment generated by system setup.");

            EnsureDocumentRecord(context, contact.Id, document1Path, document1Name, "ID", now);
            EnsureDocumentRecord(context, contact.Id, document2Path, document2Name, "Notes", now);
        }

        private static void EnsurePhotoRecord(ApplicationDbContext context, int contactId, string? virtualPath, string fileName, bool isProfile, DateTime now)
        {
            if (string.IsNullOrWhiteSpace(virtualPath))
            {
                return;
            }

            var existing = context.ContactPhotos.FirstOrDefault(p => p.ContactId == contactId && p.PhotoPath == virtualPath);
            if (existing != null)
            {
                if (isProfile && !existing.IsProfilePhoto)
                {
                    existing.IsProfilePhoto = true;
                    context.SaveChanges();
                }

                return;
            }

            var fullPath = ToPhysicalPath(virtualPath);
            var fileSize = File.Exists(fullPath) ? new FileInfo(fullPath).Length : 0;

            context.ContactPhotos.Add(new ContactPhoto
            {
                ContactId = contactId,
                PhotoPath = virtualPath,
                FileName = fileName,
                FileSize = fileSize,
                ContentType = "image/png",
                IsProfilePhoto = isProfile,
                UploadedAt = now
            });
            context.SaveChanges();
        }

        private static void EnsureDocumentRecord(ApplicationDbContext context, int contactId, string? virtualPath, string fileName, string docType, DateTime now)
        {
            if (string.IsNullOrWhiteSpace(virtualPath))
            {
                return;
            }

            var existing = context.ContactDocuments.FirstOrDefault(d => d.ContactId == contactId && d.DocumentPath == virtualPath);
            if (existing != null)
            {
                return;
            }

            var fullPath = ToPhysicalPath(virtualPath);
            var fileSize = File.Exists(fullPath) ? new FileInfo(fullPath).Length : 0;

            context.ContactDocuments.Add(new ContactDocument
            {
                ContactId = contactId,
                DocumentPath = virtualPath,
                FileName = fileName,
                FileSize = fileSize,
                ContentType = "text/plain",
                DocumentType = docType,
                UploadedAt = now
            });
            context.SaveChanges();
        }

        private static string GetUploadsRoot()
        {
            var fromEnv = Environment.GetEnvironmentVariable("UPLOADS_ROOT");
            if (!string.IsNullOrWhiteSpace(fromEnv))
            {
                return fromEnv;
            }

            var currentDirRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            return currentDirRoot;
        }

        private static string? EnsureSeedImageFile(string photosRoot, string fileName)
        {
            var targetPath = Path.Combine(photosRoot, fileName);
            if (!File.Exists(targetPath))
            {
                var sourceIcon = ResolveSeedImageSource();
                if (string.IsNullOrWhiteSpace(sourceIcon) || !File.Exists(sourceIcon))
                {
                    return null;
                }

                File.Copy(sourceIcon, targetPath, overwrite: true);
            }

            return $"/uploads/photos/{fileName}";
        }

        private static string? EnsureSeedDocumentFile(string docsRoot, string fileName, string content)
        {
            var targetPath = Path.Combine(docsRoot, fileName);
            if (!File.Exists(targetPath))
            {
                File.WriteAllText(targetPath, content);
            }

            return $"/uploads/documents/{fileName}";
        }

        private static string ToPhysicalPath(string virtualPath)
        {
            var normalized = virtualPath.Replace("\\", "/").Trim();
            if (normalized.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
            {
                var relative = normalized.Substring("/uploads/".Length).Replace('/', Path.DirectorySeparatorChar);
                return Path.Combine(GetUploadsRoot(), relative);
            }

            var relativeFromRoot = normalized.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativeFromRoot);
        }

        private static string? ResolveSeedImageSource()
        {
            var candidates = new[]
            {
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "icon-192.png"),
                Path.Combine(AppContext.BaseDirectory, "wwwroot", "icon-192.png"),
                Path.Combine(Directory.GetCurrentDirectory(), "ContactManagementAPI", "wwwroot", "icon-192.png")
            };

            return candidates.FirstOrDefault(File.Exists);
        }

        private static string? UseIfMissing(string? target, string? value)
        {
            if (string.IsNullOrWhiteSpace(target) && !string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            return target;
        }

        private static string BuildGroupMobile(int groupId)
        {
            return $"+9199000{groupId:D5}";
        }

        private sealed class SeedContactSpec
        {
            public string FirstName { get; set; } = string.Empty;
            public string? LastName { get; set; }
            public string? NickName { get; set; }
            public string? Gender { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public string? Email { get; set; }
            public string? Mobile1 { get; set; }
            public string? Mobile2 { get; set; }
            public string? Mobile3 { get; set; }
            public string? WhatsAppNumber { get; set; }
            public string? PassportNumber { get; set; }
            public string? PanNumber { get; set; }
            public string? AadharNumber { get; set; }
            public string? DrivingLicenseNumber { get; set; }
            public string? VotersId { get; set; }
            public string? Address { get; set; }
            public string? City { get; set; }
            public string? State { get; set; }
            public string? PostalCode { get; set; }
            public string? Country { get; set; }
            public int? GroupId { get; set; }
            public string? OtherDetails { get; set; }
            public string MediaTag { get; set; } = string.Empty;
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
