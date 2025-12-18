namespace OrderService.Core.DomainEvents;

public class OrderSetToPendingPaymentFailed
{
    public Guid OrderId { get; set; }
    public required string Reason { get; set; }
}