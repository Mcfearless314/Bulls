namespace OrderSagaService.Contracts.Responses.Stock;

public class StockSoldFailed
{
    public Guid OrderId { get; set; }
    public string Reason { get; set; }
}