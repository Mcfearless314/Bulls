namespace StockService.Core.DomainEvents;

public class StockCancelled
{
    public Guid OrderId { get; set; }
}