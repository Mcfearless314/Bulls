namespace StockService.Core.Contracts;

public class UpdateStockEvent
{
    public Dictionary<int, int> ProductsAndQuantities { get; set; }
    public Guid OrderId { get; set; }
    public bool IsSold { get; set; }
    public bool IsCancelled { get; set; }
}