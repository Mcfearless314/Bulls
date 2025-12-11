namespace OrderService.Core.Entities;

public class Cart
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public IEnumerable<int> ProductIds { get; set; }
}