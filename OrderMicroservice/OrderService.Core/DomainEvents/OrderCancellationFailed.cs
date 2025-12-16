namespace OrderService.Core.DomainEvents;

public class OrderCancellationFailed
{
    public Guid OrderId { get; set; }
    public required string Reason { get; set; }
}