namespace OrderSagaService.Contracts.Responses.Stock;

public class StockCancelledFailed
{
    public Guid OrderId { get; set; }
    public string Reason { get; set; }
}