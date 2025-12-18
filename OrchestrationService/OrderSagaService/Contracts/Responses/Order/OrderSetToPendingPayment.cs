namespace OrderSagaService.Contracts.Responses.Order;

public class OrderSetToPendingPayment
{
    public Guid OrderId { get; set; }
}