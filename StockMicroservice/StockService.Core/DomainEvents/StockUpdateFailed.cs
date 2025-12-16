namespace StockService.Core.DomainEvents;

public class StockUpdateFailed
{
    public int OrderId { get; set; }
    public string Reason { get; set; }
}