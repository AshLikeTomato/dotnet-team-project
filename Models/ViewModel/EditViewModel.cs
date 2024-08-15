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


        // Thêm các thuộc tính cho sản phẩm
        [Required(ErrorMessage = "Hình ảnh sản phẩm là bắt buộc.")]
        public IFormFile ProductImage { get; set; }

        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
    }
}
