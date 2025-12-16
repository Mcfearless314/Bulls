namespace OrderService.Core.DomainEvents;

public class OrderConfirmed
{
    public Guid OrderId { get; set; }
}