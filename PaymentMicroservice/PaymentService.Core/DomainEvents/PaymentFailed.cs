namespace PaymentService.Core.DomainEvents;

public class PaymentFailed
{
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public int UserId { get; set; }
    public string Reason { get; set; }
}