namespace OrderSagaService.Contracts.Commands.Order;

public class PlaceOrderEvent
{
    public Guid OrderId { get; set; }
    public int UserId { get; set; }
    public Dictionary<int, int> ProductsAndQuantities { get; set; }
}