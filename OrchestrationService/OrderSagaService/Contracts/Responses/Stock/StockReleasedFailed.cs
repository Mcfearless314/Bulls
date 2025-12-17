namespace OrderSagaService.Contracts.Responses.Stock;

public class StockReleasedFailed
{
    public Guid OrderId { get; set; }
    public required string Reason { get; set; }
}