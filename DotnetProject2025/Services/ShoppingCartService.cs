using DotnetProject2025.Models;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetProject2025.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly FirebaseClient _firebaseClient;
        private Cart _cart;

        public ShoppingCartService()
        {
            _firebaseClient = new FirebaseClient("https://dotnetproject2025-default-rtdb.asia-southeast1.firebasedatabase.app/");
        }

        private async Task InitializeCart(string userId)
        {
            try
            {
                // Retrieve the discount and other properties individually, handling null values
                var discount = await _firebaseClient
                    .Child("carts")
                    .Child(userId)
                    .Child("Discount")
                    .OnceSingleAsync<decimal?>() ?? 0; // Default to 0 if null

                var totalAmount = await _firebaseClient
                    .Child("carts")
                    .Child(userId)
                    .Child("TotalAmount")
                    .OnceSingleAsync<decimal?>() ?? 0; // Default to 0 if null

                var cartId = await _firebaseClient
                    .Child("carts")
                    .Child(userId)
                    .Child("Id")
                    .OnceSingleAsync<Guid?>() ?? Guid.NewGuid(); // Generate new ID if null

                // Initialize the cart object with these values
                _cart = new Cart
                {
                    Id = cartId,
                    UserId = userId,
                    Discount = discount,
                    TotalAmount = totalAmount,
                    Items = new List<CartItem>()
                };

                // Retrieve all items associated with the userId
                var cartItemsData = await _firebaseClient
                    .Child("carts")
                    .Child(userId)
                    .Child("items")
                    .OnceAsync<CartItem>();

                // Iterate over each item and add it to the cart's Items list
                foreach (var item in cartItemsData)
                {
                    var cartItem = item.Object;
                    if (cartItem != null)
                    {
                        _cart.Items.Add(cartItem);
                    }
                }

                Console.WriteLine("Cart initialized successfully with items and other properties.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing cart: {ex.Message}");
                _cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Items = new List<CartItem>()
                };
            }
        }


        public async Task AddProductToCartAsync(Product product, int quantity, string userId)
        {
            await InitializeCart(userId);

            var cartItem = _cart.Items.FirstOrDefault(i => i.ProductId == product.Id);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;

                // Update the specific item in Firebase
                await _firebaseClient
                    .Child("carts")
                    .Child(userId.ToString())
                    .Child("items")
                    .Child(cartItem.Id.ToString())
                    .PutAsync(cartItem);

                Console.WriteLine($"Updated quantity for product {product.Id} in cart.");
            }
            else
            {
                cartItem = new CartItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    Product = product,
                    Quantity = quantity,
                    CartId = _cart.Id
                };

                // Add the new item to the cart
                _cart.Items.Add(cartItem);

                // Save only the new item to Firebase
                await _firebaseClient
                    .Child("carts")
                    .Child(userId.ToString())
                    .Child("items")
                    .Child(cartItem.Id.ToString())
                    .PutAsync(cartItem);

                Console.WriteLine($"Added new product {product.Id} to cart.");
            }

            // Optionally update the total amount or other properties here if needed
            // _cart.TotalAmount = Cart.CalculateTotalAmount(_cart.Items, _cart.Discount);
            // await _firebaseClient.Child("carts").Child(userId.ToString()).PutAsync(_cart);
        }


        public Cart Cart
        {
            get => _cart;
            set => _cart = value;
        }

        public async Task ApplyCouponAsync(string couponCode, string userId)
        {
            await InitializeCart(userId);

            var coupons = await _firebaseClient
                .Child("coupons")
                .OrderBy("Code")
                .EqualTo(couponCode)
                .OnceAsync<Coupon>();

            var coupon = coupons.FirstOrDefault()?.Object;
            if (coupon != null && coupon.ExpiryDate > DateTime.Now)
            {
                _cart.ApplyDiscount(coupon.DiscountAmount);
                _cart.TotalAmount = Cart.CalculateTotalAmount(_cart.Items, _cart.Discount);

                // Update only the Discount and TotalAmount fields in Firebase
                await _firebaseClient
                    .Child("carts")
                    .Child(userId)
                    .Child("Discount")
                    .PutAsync(_cart.Discount);

                await _firebaseClient
                    .Child("carts")
                    .Child(userId)
                    .Child("TotalAmount")
                    .PutAsync(_cart.TotalAmount);

                Console.WriteLine($"Coupon {couponCode} applied successfully. New total: {_cart.TotalAmount}");
            }
            else
            {
                Console.WriteLine($"Coupon {couponCode} is invalid or expired.");
            }
        }


        public async Task RemoveProductFromCartAsync(int productId, string userId)
        {
            await InitializeCart(userId); 

            var cartItem = _cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (cartItem != null)
            {
                _cart.Items.Remove(cartItem);

                await _firebaseClient
                    .Child("carts")
                    .Child(userId.ToString())
                    .Child("items")
                    .Child(cartItem.Id.ToString())
                    .DeleteAsync();

                Console.WriteLine($"Removed product {productId} from cart.");
            }

            await _firebaseClient
                .Child("carts")
                .Child(userId.ToString())
                .PutAsync(_cart);
        }

        public async Task UpdateProductQuantityAsync(int productId, int quantity, string userId)
        {
            await InitializeCart(userId);

            var cartItem = _cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;

                // Update only the specific cart item in Firebase
                await _firebaseClient
                    .Child("carts")
                    .Child(userId.ToString())
                    .Child("items")
                    .Child(cartItem.Id.ToString())
                    .PutAsync(cartItem);

                Console.WriteLine($"Updated quantity for product {productId} to {quantity}.");
            }
            else
            {
                Console.WriteLine($"Product with ID {productId} not found in the cart.");
            }
        }


        public async Task ClearCartAsync(string userId)
        {
            await InitializeCart(userId); 

            foreach (var item in _cart.Items.ToList())
            {
                await _firebaseClient
                    .Child("carts")
                    .Child(userId.ToString())
                    .Child("items")
                    .Child(item.Id.ToString())
                    .DeleteAsync();
            }

            _cart.Items.Clear();

            await _firebaseClient
                .Child("carts")
                .Child(userId.ToString())
                .PutAsync(_cart);

            Console.WriteLine("Cart cleared.");
        }

        public decimal GetTotalAmount(string userId)
        {
            return _cart.TotalAmount;
        }

        public async Task<Cart> GetCartAsync(string userId)
        {
            await InitializeCart(userId);
            return _cart;
        }
    }
}
