using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AscomPayPG.Models.DTO
{
    public class UserAuthmodal
    {
        public string TransactionsKey { get; set; } = string.Empty;
        public string url { get; set; } = string.Empty;
    }
}