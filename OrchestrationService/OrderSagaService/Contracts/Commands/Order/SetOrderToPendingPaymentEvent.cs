namespace OrderSagaService.Contracts.Commands.Order;

public class SetOrderToPendingPaymentEvent
{
    public Guid OrderId { get; set; }
}