using System.ComponentModel.DataAnnotations;

namespace AscomPayPG.Models;

public class ApiEndpoint
{
    [Key]
    public int ApiId { get; set; }

	public string? Method { get; set; }

	public string? EndPoint { get; set; }

	public string? ApiName { get; set; }

	public string? ApiUser { get; set; }

	public string? ApiKey { get; set; }

	public string? AdParameter1 { get; set; }

	public string? AdParameter2 { get; set; }

	public string? AdParameter3 { get; set; }

	public string? AdParameter4 { get; set; }

	public string? AdParameter5 { get; set; }
}
