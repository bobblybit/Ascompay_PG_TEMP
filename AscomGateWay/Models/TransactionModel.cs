// DATA_ACCESS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// DATA_ACCESS.App_DB.TransactionModel
using AscomPayPG.Models.DTO;
using System.ComponentModel.DataAnnotations;


public class TransactionModel
{
    [Key]
    public long TransactionId { get; set; }
    public long? UserId { get; set; }
    public string? RequestTransactionId { get; set; }
    public bool? TransactionStatus { get; set; }
    public decimal? T_Vat { get; set; }
    public decimal? T_Charge { get; set; }
    public string? PaymentAction { get; set; }
    public string? Remark { get; set; }
    public Guid? UserUid { get; set; }

    public decimal? Amount { get; set; }

    public string? TransactionType { get; set; }

    public string? Status { get; set; }

    public int? StatusId { get; set; }

    public DateTime? Timestamp { get; set; }

    public string? SourceAccount { get; set; }

    public string? DestinationAccount { get; set; }

    public string? SourceWallet { get; set; }

    public string? DestinationWallet { get; set; }

    public User? User { get; set; }

    public string? TransactionCategory { get; set; }

    public virtual ICollection<TransactionsLog> TransactionsLogs { get; set; } = new List<TransactionsLog>();

}
