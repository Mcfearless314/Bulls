namespace OrderService.Core.DomainEvents;

public class OrderPlacingFailed
{
    public Guid OrderId { get; set; }
    public required string Reason { get; set; }
}