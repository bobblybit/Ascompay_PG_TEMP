using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AscomPayPG.Models.DTO;

namespace AscomPayPG.Models.Shared
{

    public class PaymentRequest
    {

        [Required]
        public decimal TransactionsAmount { get; set; }

        [Required]
        public string destination { get; set; }

        [Required]
        public string description { get; set; } = string.Empty;

        [Required]
        public int TransactionType { get; set; }

        [Required]
        public string Uid { get; set; }
      
        public string CallbackURL { get; set; } = null;

    }
}