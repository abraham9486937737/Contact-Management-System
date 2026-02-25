using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContactManagementAPI.Models
{
    public class UserGroup
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
        public ICollection<GroupRight> GroupRights { get; set; } = new List<GroupRight>();
    }
}
