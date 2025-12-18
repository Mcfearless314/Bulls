namespace OrderSagaService.Contracts.Responses.Order;

public class OrderCancelled
{
    public Guid OrderId { get; set; }
    public int UserId { get; set; }
    public Dictionary<int, int> ProductsAndQuantities { get; set; }
    public double Price { get; set; }
}