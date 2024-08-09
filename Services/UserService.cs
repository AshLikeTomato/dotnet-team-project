using DotnetProject2025.Models;
using Firebase.Database;
using Firebase.Database.Query;
using System.Threading.Tasks;
using System.Linq;

namespace DotnetProject2025.Services
{
    public class UserService : IUserService
    {
        private readonly FirebaseClient _firebaseClient;

        public UserService()
        {
            _firebaseClient = new FirebaseClient("https://dotnetproject2025-default-rtdb.asia-southeast1.firebasedatabase.app/");
        }

        public async Task<ApplicationUser> CreateUserAsync(string phoneNumber, string password)
        {
            var user = new ApplicationUser { UserName = phoneNumber, PhoneNumber = phoneNumber };
            await _firebaseClient
                .Child("users")
                .Child(user.UserName)
                .PutAsync(user);
            await _firebaseClient
                .Child("passwords")
                .Child(user.UserName)
                .PutAsync(password);

            return user;
        }

        public async Task<ApplicationUser> UpdateUserAsync(string userId, string newPhoneNumber, string newPassword)
        {
            var user = await _firebaseClient
                .Child("users")
                .Child(userId)
                .OnceSingleAsync<ApplicationUser>();

            if (user == null)
            {
                return null;
            }

            user.PhoneNumber = newPhoneNumber;

            await _firebaseClient
                .Child("users")
                .Child(userId)
                .PutAsync(user);

            if (!string.IsNullOrEmpty(newPassword))
            {
                await _firebaseClient
                    .Child("passwords")
                    .Child(userId)
                    .PutAsync(newPassword);
            }

            return user;
        }


        public async Task<bool> DeleteUserAsync(string userId)
        {
            var userSnapshot = await _firebaseClient
                .Child("users")
                .Child(userId)
                .OnceSingleAsync<ApplicationUser>();

            if (userSnapshot == null)
            {
                return false;
            }

            await _firebaseClient
                .Child("users")
                .Child(userId)
                .DeleteAsync();

            await _firebaseClient
                .Child("passwords")
                .Child(userId)
                .DeleteAsync();

            return true;
        }
    }
}
