using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscomPayPG.Models.DTO
{
    public class ApiPaymentRequestAuthDTO
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string? WebToken { get; set; }

        [Required]
        public int AccessType { get; set; }
    }
}
