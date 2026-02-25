using System.ComponentModel.DataAnnotations;

namespace ContactManagementAPI.Models
{
    public class GroupRight
    {
        public int Id { get; set; }

        public int UserGroupId { get; set; }
        public UserGroup? UserGroup { get; set; }

        [Required]
        [MaxLength(100)]
        public string RightKey { get; set; } = string.Empty;

        public bool IsGranted { get; set; }
    }
}
