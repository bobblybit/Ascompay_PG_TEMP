using System;
using System.ComponentModel.DataAnnotations;

namespace AscomPayPG.Models.DTO;

public class accountLookupRequest
{
    public string bank_code { get; set; }
    public string account_number { get; set; }

}

