namespace OrderService.Core.DomainEvents;

public class OrderPlacingFailed
{
    public int OrderId { get; set; }
    public required string Reason { get; set; }
}