using System.Collections.Generic;
using System.Linq;

namespace DotnetProject2025.Models
{
    public class Cart
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public decimal Discount { get; set; } = 0m;
        public string UserId { get; set; }

        public decimal TotalAmount { get; set; }

        public void RemoveItem(int productId)
        {
            var item = Items.Find(i => i.ProductId == productId);
            if (item != null)
            {
                Items.Remove(item);
            }
        }

        public void UpdateItemQuantity(int productId, int quantity)
        {
            var item = Items.Find(i => i.ProductId == productId);
            if (item != null)
            {
                item.Quantity = quantity;
            }
        }

        public void ClearItems()
        {
            Items.Clear();
        }

        public void ApplyDiscount(decimal discount)
        {
            Discount = discount;
        }

        public static decimal CalculateTotalAmount(List<CartItem> items, decimal discount)
        {
            decimal total = items.Sum(item => item.Product.Price * item.Quantity);
            decimal discountedTotal = total - discount;
            return discountedTotal < 0 ? 0 : discountedTotal;
        }
    }
}
