using DotnetProject2025.Models;
using DotnetProject2025.Services;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;

namespace DotnetProject2025.Controllers
{
    public class CartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly FirebaseClient _firebaseClient;

        public CartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
            _firebaseClient = new FirebaseClient("https://dotnetproject2025-default-rtdb.asia-southeast1.firebasedatabase.app/");
        }

        public async Task<IActionResult> Index()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _shoppingCartService.GetCartAsync(userIdString);
            var now = DateTime.Now.ToString("o");
            cart.TotalAmount = Cart.CalculateTotalAmount(cart.Items, cart.Discount);

            var coupons = await _firebaseClient
                .Child("coupons")
                .OrderBy("ExpiryDate")
                .StartAt(now)
                .OnceAsync<Coupon>();

            ViewBag.Coupons = coupons.Select(c => c.Object).ToList();
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var product = (await _firebaseClient
                    .Child("products")
                    .OrderByKey()
                    .EqualTo(productId.ToString())
                    .OnceAsync<Product>()).FirstOrDefault()?.Object;

                if (product != null)
                {
                    await _shoppingCartService.AddProductToCartAsync(product, quantity, userIdString);
                }

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> EditQuantity(int productId, int quantity)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _shoppingCartService.UpdateProductQuantityAsync(productId, quantity, userIdString);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveProduct(int productId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _shoppingCartService.RemoveProductFromCartAsync(productId, userIdString);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveAllProducts()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _shoppingCartService.ClearCartAsync(userIdString);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(string couponCode)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _shoppingCartService.ApplyCouponAsync(couponCode, userIdString);
            return RedirectToAction("Index");
        }
    }
}
