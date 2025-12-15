namespace OrderService.Core.Contracts;

public class PaymentRefunded
{
    public int OrderId { get; set; }
    public double Amount { get; set; }
    public int UserId { get; set; }
}