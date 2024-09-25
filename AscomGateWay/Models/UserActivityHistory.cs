using System;
using System.ComponentModel.DataAnnotations;

namespace AscomPayPG.Models;

public class UserActivityHistory
{
    [Key]
    public long ActivityId { get; set; }

	public long? UserId { get; set; }

	public Guid? UserUid { get; set; }

	public Guid? ActivityUid { get; set; }

	public string? ActivityType { get; set; }

	public string? ActivityDetails { get; set; }

	public DateTime? Timestamp { get; set; }

	public string? Ip { get; set; }

	public string? MacAddress { get; set; }

	public string? DeviceName { get; set; }
}
