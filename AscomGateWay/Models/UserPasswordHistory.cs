
using System;
using System.ComponentModel.DataAnnotations;

namespace AscomPayPG.Models;

public class UserPasswordHistory 
{
    [Key]
    public Guid Id { get; set; } = new Guid();
    public string UserUid { get; set; }
    public string PasswordHashString { get; set; }
    //public byte[]? PasswordSalt { get; set; }
    public byte[]? PasswordHash { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.Now;

}
