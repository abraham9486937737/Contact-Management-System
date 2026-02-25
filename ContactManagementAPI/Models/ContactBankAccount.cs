namespace ContactManagementAPI.Models
{
    public class ContactBankAccount
    {
        public int Id { get; set; }
        public int ContactId { get; set; }
        public Contact? Contact { get; set; }
        public string? AccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? BranchName { get; set; }
        public string? IfscCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
