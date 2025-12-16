namespace OrderService.Core.Contracts;

public class PaymentSucceeded
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public int UserId { get; set; }
}