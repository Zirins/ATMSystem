namespace ATMSystem.Models
{
    /// <summary>
    /// Represents a bank account in the ATM system.
    /// </summary>
    public class Account
    {
        public int AccountId { get; set; }
        public string Login { get; set; } = string.Empty;
        public string Pin { get; set; } = string.Empty;
        public string HolderName { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string Status { get; set; } = "Active";
        public string Role { get; set; } = "customer";
    }
}