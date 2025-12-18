namespace OrderSagaService.Contracts.Commands.Stock;

public class SellStockEvent
{
    public Dictionary<int, int> ProductsAndQuantities { get; set; }
    public Guid OrderId { get; set; }
}