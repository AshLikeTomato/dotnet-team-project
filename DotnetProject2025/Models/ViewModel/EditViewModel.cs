using System.ComponentModel.DataAnnotations;

namespace DotnetProject2025.Models
{
    public class EditViewModel
    {
        public string UserId { get; set; }
        [Required]
        [Phone]
        public string NewPhoneNumber { get; set; }
        
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }
        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }
    }
}
