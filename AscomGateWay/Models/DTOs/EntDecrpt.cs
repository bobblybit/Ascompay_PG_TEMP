using System.ComponentModel.DataAnnotations;

namespace AscomPayPG.Models.DTO
{
    public class EntDecrpt
    {
        [Required]
        public string Data { get; set; } = string.Empty;

        [Required]
        public int Type { get; set; } = 0;

        [Required]
        [RegularExpression("^((?!00000000-0000-0000-0000-000000000000).)*$", ErrorMessage = "Cannot use default Guid")]
        public Guid? Token { get; set; }
    }
}
