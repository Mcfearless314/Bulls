namespace OrderService.Core.Contracts;

public class AddOrderItemToOrder
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public Guid OrderId { get; set; }
}