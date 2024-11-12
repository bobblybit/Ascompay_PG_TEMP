using System;
using System.ComponentModel.DataAnnotations;

namespace AscomPayPG.Models;

public class BlueSaltBVNVerificationLog
{
    public Guid Id { get; set; } = new Guid();
    public DateTime DateCreated { get; set; } = DateTime.Now;
    public DateTime RequestTime { get; set; }
    public DateTime ResponseTime { get; set; }
    public string? CreatedBy { get; set; }
    public string? RequestPayload { get; set; }
    public string? Response { get; set; }
}
