namespace StockService.Core.DomainEvents;

public class StockReleasedFailed
{
    public int OrderId { get; set; }
    public required string Reason { get; set; }
}