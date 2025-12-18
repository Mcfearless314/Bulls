namespace OrderSagaService.Contracts.Commands.Payment;

public class PaymentRequestEvent
{
    public Guid OrderId { get; set; }
    public double Amount { get; set; }
    public int UserId { get; set; }
}