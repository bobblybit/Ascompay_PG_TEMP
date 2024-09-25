using System.ComponentModel.DataAnnotations;

namespace AscomPayPG.Models;

public class UserImage
{
    [Key]
    public long ImageId { get; set; }

	public long? UserId { get; set; }
    public Guid? UserUid { get; set; }

    public byte[]? ImageData { get; set; }

	public bool? IsVerified { get; set; }
}
