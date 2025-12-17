namespace OrderSagaService.Contracts.Responses.Payment;

public class PaymentRefundFailed
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public int UserId { get; set; }
    public string Reason { get; set; }
}