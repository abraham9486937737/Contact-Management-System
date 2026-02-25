using System;

namespace ContactManagementAPI.Models
{
    public class ContactPhoto
    {
        public int Id { get; set; }
        public int ContactId { get; set; }
        public Contact Contact { get; set; }
        public string PhotoPath { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string ContentType { get; set; }
        public bool IsProfilePhoto { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
