using System.ComponentModel.DataAnnotations;

namespace DotnetProject2025.Models
{
    public class LoginViewModel
    {
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
