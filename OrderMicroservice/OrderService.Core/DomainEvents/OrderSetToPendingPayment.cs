namespace OrderService.Core.DomainEvents;

public class OrderSetToPendingPayment
{
    public int OrderId { get; set; }
}