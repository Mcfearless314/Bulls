namespace StockService.Core.DomainEvents;

public class StockReserveFailed
{
    public Guid OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public required string Reason { get; set; }
}