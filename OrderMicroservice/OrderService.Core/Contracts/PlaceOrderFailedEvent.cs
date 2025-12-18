namespace OrderService.Core.Contracts;

public class PlaceOrderFailedEvent
{
    public Guid OrderId { get; set; }
    public bool PaymentFail { get; set; }
    public bool StockFail { get; set; }
}