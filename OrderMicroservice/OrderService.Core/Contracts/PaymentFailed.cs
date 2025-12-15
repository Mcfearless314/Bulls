namespace OrderService.Core.Contracts;

public class PaymentFailed
{
    public int OrderId { get; set; }
    public double Amount { get; set; }
    public int UserId { get; set; }
}