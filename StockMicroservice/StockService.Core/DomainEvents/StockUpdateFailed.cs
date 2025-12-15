namespace StockService.Core.DomainEvents;

public class StockUpdatedFailed
{
    public int OrderId { get; set; }
    public string Reason { get; set; }
}