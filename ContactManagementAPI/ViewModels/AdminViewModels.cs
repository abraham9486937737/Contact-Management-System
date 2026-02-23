using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContactManagementAPI.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [MaxLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }
    }

    public class UserCreateViewModel
    {
        [Required]
        [MaxLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? FullName { get; set; }

        [Required]
        public int GroupId { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UserEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? FullName { get; set; }

        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [Required]
        public int GroupId { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class GroupEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }
    }

    public class RightAssignmentViewModel
    {
        public string Key { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public bool IsGranted { get; set; }
        public string Selection { get; set; } = "Inherit";
        public bool EffectiveGranted { get; set; }
        public string EffectiveSource { get; set; } = "";
    }

    public class GroupRightsViewModel
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public List<RightAssignmentViewModel> Rights { get; set; } = new();
    }

    public class UserRightsViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? GroupName { get; set; }
        public List<RightAssignmentViewModel> Rights { get; set; } = new();
    }

    public class AdminHistoryEntryViewModel
    {
        public string ActionType { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public int? EntityId { get; set; }
        public string Details { get; set; } = string.Empty;
        public string PerformedBy { get; set; } = string.Empty;
        public DateTime PerformedAt { get; set; }
    }

    public class AdminHistoryListViewModel
    {
        public List<AdminHistoryEntryViewModel> Entries { get; set; } = new();
    }
}
