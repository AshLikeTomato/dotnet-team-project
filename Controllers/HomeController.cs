using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DotnetProject2025.Models;
using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace DotnetProject2025.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FirebaseClient _firebaseClient;
        private static readonly HttpClient client = new HttpClient();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _firebaseClient = new FirebaseClient("https://dotnetproject2025-default-rtdb.asia-southeast1.firebasedatabase.app/");
        }

        public async Task<IActionResult> Index()
        {
            var firebaseUrl = "https://dotnetproject2025-default-rtdb.asia-southeast1.firebasedatabase.app/products.json";

            var jsonResponse = await client.GetStringAsync(firebaseUrl);

            // Deserialize JSON object thành Dictionary<string, Product>
            var productsDict = JsonConvert.DeserializeObject<Dictionary<string, Product>>(jsonResponse);

            // Chuyển đổi Dictionary thành List<Product>
            var productList = productsDict.Values.ToList();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["IsAdmin"] = userId == "yKy1WrjEXOTBPDV5W7EfosdGQJQ2";

            return View(productList);
        }



        public async Task<IActionResult> SeedData()
        {
            var sampleProducts = new List<Product>
    {
        new Product { Id = 1, Name = "Product 1", Price = 10.0m },
        new Product { Id = 2, Name = "Product 2", Price = 20.0m },
        new Product { Id = 3, Name = "Product 3", Price = 30.0m },
        new Product { Id = 4, Name = "Product 4", Price = 40.0m },
        new Product { Id = 5, Name = "Product 5", Price = 50.0m }
    };

            foreach (var product in sampleProducts)
            {
                await _firebaseClient
                    .Child("products")
                    .Child(product.Id.ToString())
                    .PutAsync(product);
            }

            var sampleCoupons = new List<Coupon>
    {
        new Coupon { Id = 1, Code = "DISCOUNT10", DiscountAmount = 10.0m, ExpiryDate = DateTime.Now.AddMonths(1) },
        new Coupon { Id = 2, Code = "DISCOUNT20", DiscountAmount = 20.0m, ExpiryDate = DateTime.Now.AddMonths(2) },
        new Coupon { Id = 3, Code = "DISCOUNT30", DiscountAmount = 30.0m, ExpiryDate = DateTime.Now.AddMonths(3) }
    };

            foreach (var coupon in sampleCoupons)
            {
                await _firebaseClient
                    .Child("coupons")
                    .Child(coupon.Id.ToString())
                    .PutAsync(coupon);
            }

            return Ok("Sample data seeded successfully");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
