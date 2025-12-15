namespace StockService.Core.Contracts;

public class ReserveProductEvent
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public int OrderId { get; set; }
}