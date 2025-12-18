namespace OrderService.Core.DomainEvents;

public class RemovedProductFromOrderItems
{
    public Guid OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}