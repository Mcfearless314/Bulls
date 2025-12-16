namespace OrderService.Core.DomainEvents;

public class OrderCancellationFailed
{
    public int OrderId { get; set; }
    public required string Reason { get; set; }
}