using DotnetProject2025.Models;
using System;
using System.Threading.Tasks;

namespace DotnetProject2025.Services
{
    public interface IShoppingCartService
    {
        Task<Cart> GetCartAsync(string userId);
        Task AddProductToCartAsync(Product product, int quantity, string userId);
        Task RemoveProductFromCartAsync(int productId, string userId);
        Task UpdateProductQuantityAsync(int productId, int quantity, string userId);
        Task ClearCartAsync(string userId);
        Task ApplyCouponAsync(string couponCode, string userId);
        decimal GetTotalAmount(string userId);
    }
}
