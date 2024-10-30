using System.ComponentModel.DataAnnotations;

namespace AscomPayPG.Models.DTO
{
    public class ApiUserLogin
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^((?!00000000-0000-0000-0000-000000000000).)*$", ErrorMessage = "Cannot use default Guid")]
        public Guid? ProductUID { get; set; }

        [Required]
        [RegularExpression("^((?!00000000-0000-0000-0000-000000000000).)*$", ErrorMessage = "Cannot use default Guid")]
        public Guid? Database { get; set; }
    }
}
