namespace OrderService.Core.DomainEvents;

public class OrderConfirmFailed
{
    public int OrderId { get; set; }
    public required string Reason { get; set; }
}