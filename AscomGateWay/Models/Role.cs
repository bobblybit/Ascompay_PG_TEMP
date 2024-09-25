using System;
using System.ComponentModel.DataAnnotations;

namespace AscomPayPG.Models;

public class Role
{
    [Key]
    public int RoleId { get; set; }

	public Guid? RoleUid { get; set; }

	public string? RoleName { get; set; }

	public int? Status { get; set; }
}
