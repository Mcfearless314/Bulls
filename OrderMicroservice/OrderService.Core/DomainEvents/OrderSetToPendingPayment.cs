namespace OrderService.Core.DomainEvents;

public class OrderSetToPendingPayment
{
    public Guid OrderId { get; set; }
}