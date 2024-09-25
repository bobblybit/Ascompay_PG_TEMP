using AscomPayPG.Models.Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace AscomPayPG.Models.DTO;

public class TransactionsLog :BaseEntity
{
    [Key]
    public long LogId { get; set; }
	public long? UserId { get; set; }

	public Guid? UserUid { get; set; }
    public long TransactionId { get; set; }
	public string? Action { get; set; }

	public string? LogCategory { get; set; }

	public string? Details { get; set; }

	public DateTime? Timestamp { get; set; }

	public virtual Transactions? Transaction { get; set; }
}
