namespace OrderService.Core.Entities;

public class OrderItem
{
    public int Id { get; set; } 
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public Guid OrderId { get; set; }
    public int Quantity { get; set; }
}