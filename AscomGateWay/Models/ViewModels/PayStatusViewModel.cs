using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AscomPayPG.Models.ViewModels
{
    public class PayStatusViewModel
    {
        public bool Status { get; set; }
        public string? Reference { get; set; }
        public string? StatusDescription { get; set; }
        public string? CallbackURL { get; set; }

    }
}