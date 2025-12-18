namespace OrderSagaService.Contracts.Responses.Order;

public class OrderPlacingFailed
{
    public Guid OrderId { get; set; }
    public required string Reason { get; set; }
}