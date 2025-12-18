namespace OrderSagaService.Contracts.Commands.Stock;

public class CancelStockEvent
{
    public Dictionary<int, int> ProductsAndQuantities { get; set; }
    public Guid OrderId { get; set; }
}