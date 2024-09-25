using System;
using System.ComponentModel.DataAnnotations;

namespace AscomPayPG.Models;

public class OTP
{
    [Key]
    public Guid Id { get; set; }

	public string UserUID { get; set; }

	public string? Code { get; set; }

	public string? Name { get; set; }

	public string? CreatedBy { get; set; }

	public string? UpdatedBy { get; set; }

	public DateTime Expires { get; set; }

	public DateTime CreatedAt { get; set; }

	public DateTime UpdatedAt { get; set; }
}
