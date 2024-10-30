using System.ComponentModel.DataAnnotations;

namespace AscomPayPG.Models.DTO
{
    public class TransactionType
    {
        [Key]
        public int SN { get; set; }
        public int TiD { get; set; }
        public string Ttype { get; set; }
        public decimal T_Percentage { get; set; }
        public decimal T_Amount { get; set; }
        public decimal T_Vat { get; set; }
        public bool By_Percent { get; set; }
        public bool By_Amount { get; set; }
        public bool Add_Vat { get; set; }
        public int? T_Action { get; set; }
        public string? T_Status { get; set; }
        public int? T_Channel { get; set; }
    }
}
