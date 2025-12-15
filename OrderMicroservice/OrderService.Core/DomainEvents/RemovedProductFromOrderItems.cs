namespace OrderService.Core.DomainEvents;

public class ProductRemovedFromOrderItems
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}