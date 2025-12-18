namespace OrderService.Core.DomainEvents;

public class OrderCancelled
{
    public Guid OrderId { get; set; }
    public int UserId { get; set; }
    public Dictionary<int, int> ProductsAndQuantities { get; set; }
}