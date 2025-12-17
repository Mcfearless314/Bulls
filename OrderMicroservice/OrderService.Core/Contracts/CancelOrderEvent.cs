namespace OrderService.Core.Contracts;

public class CancelOrderEvent
{
    public Guid OrderId { get; set; }
    public int UserId { get; set; }
    public Dictionary<int, int> ProductsAndQuantities { get; set; }
}