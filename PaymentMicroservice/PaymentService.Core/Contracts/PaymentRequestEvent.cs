namespace PaymentService.Core.Contracts;

public class PaymentRequestEvent
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public int UserId { get; set; }
}