using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContactManagementAPI.Models
{
    public class AppUser
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? FullName { get; set; }

        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; } = true;

        [Required]
        public int GroupId { get; set; }
        public UserGroup? Group { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<UserRight> UserRights { get; set; } = new List<UserRight>();
    }
}
