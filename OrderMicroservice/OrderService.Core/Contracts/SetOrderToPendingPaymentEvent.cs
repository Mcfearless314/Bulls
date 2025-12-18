namespace OrderService.Core.Contracts;

public class SetOrderToPendingPaymentEvent
{
    public Guid OrderId { get; set; }
}