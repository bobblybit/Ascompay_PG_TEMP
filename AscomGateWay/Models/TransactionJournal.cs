using AscomPayPG.Models.Shared;
using System.ComponentModel.DataAnnotations;

namespace AscomPayPG.Models
{
    public class TransactionJournal : BaseEntity
    {
        public TransactionJournal()
        {
            //Status = false;
            Status = PaymentStatus.Pending.ToString();
            Timestamp = DateTime.Now;
        }

        [Key]
        public string? TransactionReference { get; set; }
        public string? SenderName { get; set; }
        public string? RecieverName { get; set; }
        public string? UserUID { get; set; } //UserUID request id 
        public decimal Amount { get; set; }
        public string? TransactionType { get; set; }
        public string? Status { get; set; }
        public DateTime Timestamp { get; set; }
        public string? AccountOrWalletId { get; set; }
        public string? JournalType { get; set; }
        public string? PaymentProvider { get; set; }
        public string? Description { get; set; }
        public string? PaymentAction { get; set; }
        public string? AccessToken { get; set; }
        public string? RequestTransactionId { get; set; }
        public decimal T_Charge { get; set; }
        public decimal T_Vat { get; set; }
        public decimal T_Provider_Charges { get; set; }
        public decimal T_Marchant_Charges { get; set; }
        public string NetAmount { get; set; }
        public int BankCode { get; set; }
        public string Currency { get; set; }
    }
}
