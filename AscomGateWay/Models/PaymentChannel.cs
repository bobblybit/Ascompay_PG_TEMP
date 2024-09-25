using System;
using System.ComponentModel.DataAnnotations;

namespace AscomPayPG.Models;

public class PaymentChannel
{
    [Key]
    public int ChannelId { get; set; }

	public string? ChannelName { get; set; }

	public string? ChannelType { get; set; }

	public string? AccountDetails { get; set; }

	public bool? IsActive { get; set; }

	public Guid? ChannelUid { get; set; }
}
