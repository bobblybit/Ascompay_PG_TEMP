using System.ComponentModel.DataAnnotations;

namespace AscomPayPG.Models;

public class EncryptedPayment
{
    [Key]
    public int PaymentId { get; set; }

	public long? UserId { get; set; }
    public Guid? UserUid { get; set; }

    public string? EncryptedData { get; set; }

	public string? QrcodeUrl { get; set; }

	public bool? IsDecrypted { get; set; }
}
