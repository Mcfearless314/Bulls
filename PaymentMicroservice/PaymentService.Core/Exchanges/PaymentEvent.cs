namespace PaymentService.Core.Exchanges;

public class PaymentEvent
{
    // Contracts
    public const string PaymentRequestEvent = "payment_request_event";
    public const string PaymentRefundEvent = "payment_refund_event";
    
    // Domain Events
    public const string PaymentSucceeded = "payment_succeeded";
    public const string PaymentFailed = "payment_failed";
    public const string PaymentRefunded = "payment_refunded";
    public const string PaymentRefundFailed = "payment_refund_failed";
}