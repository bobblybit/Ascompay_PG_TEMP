using AscomPayPG.Models.DTO;
using AscomPayPG.Models.Shared;
using System.ComponentModel.DataAnnotations;

namespace AscomPayPG.Models
{
    public class Transactions
    {
        public Transactions()
        {
            //Status = false;
            Status = PaymentStatus.Pending.ToString();
            StatusId = 2;

            UpdatedAt = DateTime.Now;
            Timestamp = DateTime.Now;
            PaymentGateway = new PaymentGateway();
            User = new List<User>();
            TransactionsLogs = new List<TransactionsLog>();
        }

        [Key]
        public long TransactionId { get; set; }
        public string ?SenderName { get; set; }
        public string? RecieverName { get; set; }
        public long? UserId { get; set; }
        public Guid? UserUID { get; set; } //UserUID request id 
        public decimal Amount { get; set; }
        public string? TransactionType { get; set; }
        public string? Status { get; set; }
        public int? StatusId { get; set; }
        public DateTime Timestamp { get; set; }
        public string? SourceAccount { get; set; }
        public string? DestinationAccount { get; set; }
        public string? SourceWallet { get; set; }
        public string? DestinationWallet { get; set; }
        public string? TransactionCategory { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public long? PaymentGatewayId { get; set; }
        public string? Description { get; set; }
        public string? Email { get; set; }
        public bool? TransactionStatus { get; set; }
        public string? CallbackURL { get; set; }
        public string? PaymentAction { get; set; }
        public string? PaymentProvider { get; set; }
        public string? AccessToken { get; set; }
        public string? RequestTransactionId { get; set; }
        public decimal T_Charge { get; set; }
        public decimal T_Vat { get; set; }
        public decimal T_Provider_Charges { get; set; }
        public decimal T_Marchant_Charges { get; set; }
        public decimal NetAmount { get; set; }
        public int BankCode { get; set; }
        public string Currency { get; set; }
        public virtual ICollection<User> User { get; set; } = new List<User>();
        public virtual PaymentGateway PaymentGateway { get; set; }
        public virtual ICollection<TransactionsLog> TransactionsLogs { get; set; } = new List<TransactionsLog>();
    }
}