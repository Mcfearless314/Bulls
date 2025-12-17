namespace OrderSagaService.Contracts.Commands.Order;

public class PlaceOrderFailedEvent
{
    public Guid OrderId { get; set; }
    public bool PaymentFail { get; set; }
    public bool StockFail { get; set; }
}