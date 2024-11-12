using System;
using System.ComponentModel.DataAnnotations;

namespace AscomPayPG.Models;

public class UserKyc
{
    [Key]
    public long Kycid { get; set; }

    public long? UserId { get; set; }
    public Guid? UserUid { get; set; }
    public string? DocumentType { get; set; }
    public string? CompanyName { get; set; }
    public string? DocumentNumber { get; set; }
    public string? FrontImage { get; set; }
    public string? BackImage { get; set; }
    public string? SelfieImage { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool? IsVerified { get; set; }
    public DateTime? DateCreated { get; set; } = DateTime.Now;
    public DateTime? LastUpdated { get; set; }
    public bool IsBvnVerified { get; set; }
    public string? BvnVerifiedName { get; set; }
    public string? BlueSaltBVNVerificationResponse { get; set; }
}
