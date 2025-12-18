
namespace StockService.Core.DomainEvents;

public class StockReserved
{
    public Guid OrderId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
}