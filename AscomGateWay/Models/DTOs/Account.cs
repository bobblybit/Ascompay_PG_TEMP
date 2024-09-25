using System;
using System.ComponentModel.DataAnnotations;

namespace AscomPayPG.Models.DTO;

public class Account
{
    [Key]
    public long AccountId { get; set; }

    public long? UserId { get; set; }

    public Guid? UserUid { get; set; }

    public string? AccountName { get; set; }

    public string? AccountNumber { get; set; }
    public string? PhoneNumber { get; set; }

    public string? Bvn { get; set; }

    public string? Nin { get; set; }

    public bool? IsPrimary { get; set; }

    public decimal? CurrentBalance { get; set; }
    public decimal? LegerBalance { get; set; }
    public decimal? PrevioseBalance { get; set; }

    public DateTime? LastUpdated { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public DateTime? AccountOpeningDate { get; set; }
}

