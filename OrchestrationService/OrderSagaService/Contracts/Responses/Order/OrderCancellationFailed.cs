namespace OrderSagaService.Contracts.Responses.Order;

public class OrderCancellationFailed
{
    public Guid OrderId { get; set; }
    public required string Reason { get; set; }
}