namespace StockService.Core.Contracts;

public class FreeProductReservationEvent
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public int OrderId { get; set; }
}