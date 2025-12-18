namespace OrderSagaService.Contracts.Responses.Order;

public class ProductRemovedFromOrderItems
{
    public Guid OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}