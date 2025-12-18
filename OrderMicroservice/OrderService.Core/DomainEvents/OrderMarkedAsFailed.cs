namespace OrderService.Core.DomainEvents;

public class OrderMarkedAsFailed
{
    public Guid OrderId { get; set; }
    public bool Success { get; set; }
}