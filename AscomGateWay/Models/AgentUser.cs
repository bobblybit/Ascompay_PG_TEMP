
using AscomPayPG.Models.DTO;
using AscomPayPG.Models.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AscomPayPG.Models;

public class AgentUser : BaseEntity
{
    [Key]
    public long UserId { get; set; }

    public Guid? UserUid { get; set; }

    public string? Username { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? MiddleName { get; set; }

    public string? Email { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public byte[]? PasswordHash { get; set; }

    public byte[]? PasswordSalt { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public DateTime? RegistrationDate { get; set; }

    public string? UserType { get; set; }

    public int? Accountstatus { get; set; }

    public int LoginAttempted { get; set; }

    public DateTime LoginTimestamp { get; set; }

    public bool? IsEmailVerified { get; set; }

    public string? ResetPasswordCode { get; set; }

    public Guid? ActivationCode { get; set; }

    public int? RoleId { get; set; }

    public string? Ip { get; set; }

    public string? CurrentMacAddress { get; set; }

    public string? CurrentDevice { get; set; }

    public bool? OnlineStaus { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public bool? IsNotificationEnabled { get; set; }
    public bool? IsKycVerified { get; set; }
    public string? CompanyCode { get; set; }
    public string? UserCode { get; set; }
    public DateTime? DateCreated { get; set; } = DateTime.Now;
    public DateTime? LastUpdated { get; set; }
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();


    public virtual ICollection<EncryptedPayment> EncryptedPayments { get; set; } = new List<EncryptedPayment>();


    public virtual ICollection<TransactionModel> Transactions { get; set; } = new List<TransactionModel>();


    public virtual ICollection<TransactionsLog> TransactionsLogs { get; set; } = new List<TransactionsLog>();


    public virtual ICollection<UserActivityHistory> UserActivityHistories { get; set; } = new List<UserActivityHistory>();


    public virtual ICollection<UserImage> UserImages { get; set; } = new List<UserImage>();


    public virtual ICollection<UserKyc> UserKycs { get; set; } = new List<UserKyc>();


    public virtual ICollection<UserWallet> UserWallets { get; set; } = new List<UserWallet>();

}