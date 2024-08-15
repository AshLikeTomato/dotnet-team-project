using Microsoft.AspNetCore.Identity;

namespace DotnetProject2025.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Phone {  get; set; }
    }
}
