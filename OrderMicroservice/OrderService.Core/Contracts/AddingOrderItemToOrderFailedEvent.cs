namespace OrderService.Core.Contracts;

public class AddingOrderItemToOrderFailedEvent
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public Guid OrderId { get; set; }
    public required string Reason { get; set; }
}