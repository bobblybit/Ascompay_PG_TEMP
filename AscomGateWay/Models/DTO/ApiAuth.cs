using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscomPayPG.Models.DTO
{
    public class ApiAuth
    {
        [Required]
        [FromHeader]
        public string TransactionsKey { get; set; }
    }
}
