using DotnetProject2025.Models;
using System.Threading.Tasks;

namespace DotnetProject2025.Services
{
    public interface IUserService
    {
        Task<ApplicationUser> CreateUserAsync(string phoneNumber, string password);
        Task<ApplicationUser> UpdateUserAsync(string userId, string newPhoneNumber, string newPassword);
        Task<bool> DeleteUserAsync(string userId);
    }
}
