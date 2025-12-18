namespace OrderSagaService.Contracts.Responses.Stock;

public class StockReserveFailed
{
    public Guid OrderId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public required string Reason { get; set; }
}