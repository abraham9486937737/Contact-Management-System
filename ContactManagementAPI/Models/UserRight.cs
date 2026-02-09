using System.ComponentModel.DataAnnotations;

namespace ContactManagementAPI.Models
{
    public class UserRight
    {
        public int Id { get; set; }

        public int AppUserId { get; set; }
        public AppUser? AppUser { get; set; }

        [Required]
        [MaxLength(100)]
        public string RightKey { get; set; } = string.Empty;

        public bool IsGranted { get; set; }
    }
}
