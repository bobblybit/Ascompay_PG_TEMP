using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AscomPayPG.Models;
//[NotMapped]
//public class TransactionModel 
//{
//    [Key]
//    public long TransactionId { get; set; }
//	public long? UserId { get; set; }
//	public Guid? UserUid { get; set; }
//	public decimal? Amount { get; set; }
//	public string? TransactionType { get; set; }
//	public string? Status { get; set; }
//	public int? StatusId { get; set; }
//	public DateTime? Timestamp { get; set; }	
//	public string? SourceAccount { get; set; }
//	public string? DestinationAccount { get; set; }
//    public string? SourceWallet { get; set; }
//    public string? DestinationWallet { get; set; }
//    public User? User { get; set; }
//	public string? TransactionCategory { get; set; }
//	public virtual ICollection<TransactionsLog> TransactionsLogs { get; set; } = new List<TransactionsLog>();
//}
