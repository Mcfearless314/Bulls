namespace PaymentService.Core.DomainEvents;

public class PaymentRefunded
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public int UserId { get; set; }
}