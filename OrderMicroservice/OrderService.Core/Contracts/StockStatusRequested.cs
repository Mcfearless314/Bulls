namespace OrderService.Core.Contracts;

public class StockStatusRequested
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}