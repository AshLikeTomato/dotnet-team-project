using DotnetProject2025.Models;
using System.ComponentModel.DataAnnotations;

public class CartItem
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid CartId { get; set; }
    public Cart Cart { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public int Quantity { get; set; }
}
