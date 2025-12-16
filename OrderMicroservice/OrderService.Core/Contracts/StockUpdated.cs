namespace OrderService.Core.Contracts;

public class StockUpdated
{
    public Guid OrderId { get; set; }
    public bool? IsSold { get; set; }
    public bool? IsReleased { get; set; }
}