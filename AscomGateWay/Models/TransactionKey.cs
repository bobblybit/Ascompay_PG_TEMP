using System;
using System.ComponentModel.DataAnnotations;

namespace AscomPayPG.Models;

public class TransactionKey
{
    [Key]
    public int KeyId { get; set; }

	public Guid? Token { get; set; }

	public Guid? UserUid { get; set; }

	public byte[]? PasswordHash { get; set; }

	public byte[]? PasswordSalt { get; set; }
}
