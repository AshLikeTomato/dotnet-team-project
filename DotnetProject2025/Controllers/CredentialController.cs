using DotnetProject2025.Models;
using FirebaseAdmin.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using Google.Type;

namespace DotnetProject2025.Controllers
{
    [Route("credential")]
    public class CredentialController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly FirebaseClient _firebaseClient;

        public CredentialController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _firebaseClient = new FirebaseClient("https://dotnetproject2025-default-rtdb.asia-southeast1.firebasedatabase.app/");
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] dynamic data)
        {
            try
            {
                string idToken = data.GetProperty("token").GetString();
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                string uid = decodedToken.Uid;
                string email = decodedToken.Claims["email"].ToString();

                var user = await GetUserByEmailAsync(email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        Id = uid,
                        UserName = email,
                        Email = email
                    };
                    await CreateUserAsync(user);
                }

                await SignInUser(user);

                var response = new { success = true, message = "User logged in successfully." };
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Google login error: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            var users = await _firebaseClient
                .Child("users")
                .OrderBy("Email")
                .EqualTo(email)
                .OnceAsync<ApplicationUser>();

            return users.FirstOrDefault()?.Object;
        }

        private async Task CreateUserAsync(ApplicationUser user)
        {
            await _firebaseClient
                .Child("users")
                .Child(user.Id)
                .PutAsync(user);
        }

        private async Task SignInUser(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = System.DateTime.UtcNow.AddMinutes(60)
            });

            // Set cookie
            HttpContext.Response.Cookies.Append("LoginGoogle", "true", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string phoneNumber, string password)
        {
            phoneNumber = new string(phoneNumber.Where(char.IsDigit).ToArray());

            if (!phoneNumber.StartsWith("+"))
            {
                phoneNumber = $"+1{phoneNumber}";
            }

            var user = await GetUserByPhoneNumberAsync(phoneNumber);
            if (user != null)
            {
                var passwordVerificationResult = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

                if (passwordVerificationResult == PasswordVerificationResult.Success)
                {
                    await SignInUser(user);
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Login failed. Please try again.");
            return View();
        }



        private async Task<ApplicationUser> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            var users = await _firebaseClient
                .Child("users")
                .OnceAsync<ApplicationUser>();

            foreach (var user in users)
            {
                if (user.Object.Phone == phoneNumber)
                {
                    return user.Object;
                }
            }
            return null;
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(string phoneNumber, string password, string address)
        {
            try
            {
                phoneNumber = new string(phoneNumber.Where(char.IsDigit).ToArray());

                if (!phoneNumber.StartsWith("+"))
                {
                    phoneNumber = $"+1{phoneNumber}";
                }

                UserRecordArgs userRecordArgs = new UserRecordArgs()
                {
                    PhoneNumber = phoneNumber,
                    Password = password,
                    DisplayName = phoneNumber,
                };

                UserRecord userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userRecordArgs);

                var user = new ApplicationUser
                {
                    Id = userRecord.Uid,
                    UserName = phoneNumber,
                    Phone = phoneNumber,
                };

                // Hash the password before storing it
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, password);

                await CreateUserAsync(user);

                await SignInUser(user);

                return RedirectToAction("Index", "Home");
            }
            catch (FirebaseAuthException ex)
            {
                ModelState.AddModelError(string.Empty, $"Registration failed: {ex.Message}");
            }

            return View();
        }
        [HttpPost("/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("/edit")]
        public async Task<IActionResult> Edit(string userId)
        {
            var user = await _firebaseClient
                .Child("users")
                .Child(userId)
                .OnceSingleAsync<ApplicationUser>();

            if (user == null)
            {
                return NotFound();
            }

            var model = new EditViewModel
            {
                UserId = user.Id,
                NewPhoneNumber = user.Phone,
                Email = user.Email,
            };

            return View(model);
        }

        [HttpPost("/edit")]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var updateData = new
                {
                    Email = model.Email,
                    PhoneNumber = model.NewPhoneNumber
                };

                await _firebaseClient
                    .Child("users")
                    .Child(model.UserId)
                    .PatchAsync(updateData);

                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    var user = new ApplicationUser
                    {
                        Id = model.UserId
                    };

                    user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.NewPassword);

                    var passwordUpdateData = new
                    {
                        PasswordHash = user.PasswordHash
                    };

                    await _firebaseClient
                        .Child("users")
                        .Child(model.UserId)
                        .PatchAsync(passwordUpdateData);
                }

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }


        // Delete Account
        [HttpPost("delete")]
        public async Task<IActionResult> Delete()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _firebaseClient
                .Child("users")
                .Child(userId)
                .DeleteAsync();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> LogoutGoogle()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            return RedirectToAction("Index", "Home");
        }

        private async Task AddClaimsToUser(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, "User")
            };

            await _userManager.AddClaimsAsync(user, claims);
        }
    }
}
