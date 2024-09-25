using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscomPayPG.Models.DTO
{
    public class KeyWord
    {
        [Required]
        public string Word { get; set; }
    }
}
