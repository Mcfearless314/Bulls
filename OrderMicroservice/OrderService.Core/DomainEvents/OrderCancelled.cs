namespace OrderService.Core.DomainEvents;

public class OrderCancelled
{
    public Guid OrderId { get; set; }
}