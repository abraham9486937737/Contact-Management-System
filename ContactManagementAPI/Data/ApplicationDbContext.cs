using Microsoft.EntityFrameworkCore;
using ContactManagementAPI.Models;

namespace ContactManagementAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactGroup> ContactGroups { get; set; }
        public DbSet<ContactPhoto> ContactPhotos { get; set; }
        public DbSet<ContactDocument> ContactDocuments { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<GroupRight> GroupRights { get; set; }
        public DbSet<UserRight> UserRights { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Contact - ContactGroup relationship
            modelBuilder.Entity<Contact>()
                .HasOne(c => c.Group)
                .WithMany(g => g.Contacts)
                .HasForeignKey(c => c.GroupId)
                .OnDelete(DeleteBehavior.SetNull);

            // Contact - ContactPhoto relationship
            modelBuilder.Entity<ContactPhoto>()
                .HasOne(cp => cp.Contact)
                .WithMany(c => c.Photos)
                .HasForeignKey(cp => cp.ContactId)
                .OnDelete(DeleteBehavior.Cascade);

            // Contact - ContactDocument relationship
            modelBuilder.Entity<ContactDocument>()
                .HasOne(cd => cd.Contact)
                .WithMany(c => c.Documents)
                .HasForeignKey(cd => cd.ContactId)
                .OnDelete(DeleteBehavior.Cascade);

            // AppUser - UserGroup relationship
            modelBuilder.Entity<AppUser>()
                .HasOne(u => u.Group)
                .WithMany(g => g.Users)
                .HasForeignKey(u => u.GroupId)
                .OnDelete(DeleteBehavior.SetNull);

            // GroupRight - UserGroup relationship
            modelBuilder.Entity<GroupRight>()
                .HasOne(gr => gr.UserGroup)
                .WithMany(g => g.GroupRights)
                .HasForeignKey(gr => gr.UserGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserRight - AppUser relationship
            modelBuilder.Entity<UserRight>()
                .HasOne(ur => ur.AppUser)
                .WithMany(u => u.UserRights)
                .HasForeignKey(ur => ur.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<UserGroup>()
                .HasIndex(g => g.Name)
                .IsUnique();

            modelBuilder.Entity<GroupRight>()
                .HasIndex(gr => new { gr.UserGroupId, gr.RightKey })
                .IsUnique();

            modelBuilder.Entity<UserRight>()
                .HasIndex(ur => new { ur.AppUserId, ur.RightKey })
                .IsUnique();

            // Seed default contact groups
            modelBuilder.Entity<ContactGroup>().HasData(
                new ContactGroup { Id = 1, Name = "Family", Description = "Family members", CreatedAt = DateTime.Now },
                new ContactGroup { Id = 2, Name = "Friends", Description = "Friends", CreatedAt = DateTime.Now },
                new ContactGroup { Id = 3, Name = "Business", Description = "Business contacts", CreatedAt = DateTime.Now },
                new ContactGroup { Id = 4, Name = "School", Description = "School contacts", CreatedAt = DateTime.Now },
                new ContactGroup { Id = 5, Name = "Church", Description = "Church members", CreatedAt = DateTime.Now },
                new ContactGroup { Id = 6, Name = "Others", Description = "Other contacts", CreatedAt = DateTime.Now }
            );
        }
    }
}
