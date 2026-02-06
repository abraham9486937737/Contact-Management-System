using System;
using System.Collections.Generic;

namespace ContactManagementAPI.Models
{
    public class ContactGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    }
}
