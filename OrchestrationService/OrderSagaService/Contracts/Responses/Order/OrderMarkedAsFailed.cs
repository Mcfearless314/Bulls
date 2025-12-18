namespace OrderSagaService.Contracts.Responses.Order;

public class OrderMarkedAsFailed
{
    public Guid OrderId { get; set; }
    public bool Success { get; set; }
}