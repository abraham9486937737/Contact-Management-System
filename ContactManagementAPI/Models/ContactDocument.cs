using System;

namespace ContactManagementAPI.Models
{
    public class ContactDocument
    {
        public int Id { get; set; }
        public int ContactId { get; set; }
        public Contact Contact { get; set; }
        public string DocumentPath { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string ContentType { get; set; }
        public string DocumentType { get; set; } // e.g., "ID", "Address", "Contract", etc.
        public DateTime UploadedAt { get; set; }
    }
}
