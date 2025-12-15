
namespace StockService.Core.DomainEvents;

public class StockStatusRequested
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
}