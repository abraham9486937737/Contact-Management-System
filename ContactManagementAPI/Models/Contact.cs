using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContactManagementAPI.Models
{
    public class Contact
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string? NickName { get; set; }
        public string? Email { get; set; }
        public string? Mobile1 { get; set; }
        public string? Mobile2 { get; set; }
        public string? Mobile3 { get; set; }
        public string? WhatsAppNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string? PhotoPath { get; set; }
        public int? GroupId { get; set; }
        public ContactGroup? Group { get; set; }
        public string? OtherDetails { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<ContactPhoto> Photos { get; set; } = new List<ContactPhoto>();
        public ICollection<ContactDocument> Documents { get; set; } = new List<ContactDocument>();
    }
}
