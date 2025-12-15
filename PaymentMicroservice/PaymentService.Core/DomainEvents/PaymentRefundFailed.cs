namespace PaymentService.Core.DomainEvents;

public class PaymentRefundFailed
{
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public int UserId { get; set; }
    public string Reason { get; set; }
}