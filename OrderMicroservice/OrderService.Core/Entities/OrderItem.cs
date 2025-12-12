namespace OrderService.Core.Entities;

public class OrderItem
{
    public int ProductId { get; set; }
    public int OrderId { get; set; }
    public int Quantity { get; set; }
}