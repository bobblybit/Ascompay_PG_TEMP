using System.ComponentModel.DataAnnotations;

namespace AscomPayPG.Models;

public class EnKey
{
    [Key]
    public int Ukey { get; set; }
	public string? kName { get; set; }
	public string? eKey { get; set; }
	public int? Type { get; set; }
	public bool? Status { get; set; }
}
