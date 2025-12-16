namespace StockService.Core.DomainEvents;

public class StockUpdateFailed
{
    public Guid OrderId { get; set; }
    public string Reason { get; set; }
}