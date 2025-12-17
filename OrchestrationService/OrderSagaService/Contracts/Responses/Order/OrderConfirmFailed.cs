namespace OrderSagaService.Contracts.Responses.Order;

public class OrderConfirmFailed
{
    public Guid OrderId { get; set; }
    public required string Reason { get; set; }
}