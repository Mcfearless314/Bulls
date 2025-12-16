namespace PaymentService.Core.Contracts;

public class PaymentRefundEvent
{
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public int UserId { get; set; }
}