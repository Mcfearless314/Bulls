namespace OrderSagaService.State;

public class OrderSagaState
{
    public Guid SagaId { get; set; }
    public Guid OrderId { get; set; }
    public int UserId { get; set; }
    public ActionType Action { get; set; }
    public Dictionary<int, int> ProductsAndQuantities { get; set; }
    public double Price { get; set; }
    public int ConfirmRetryCount { get; set; } = 0;
    public int RefundRetryCount { get; set; } = 0;
    public int StockRetryCount { get; set; }
}

public enum ActionType
{
    OrderConfirmed = 0,
    OrderPlaced = 1,
    StockSold = 2,
    StockSoldFailed = 3,
    OrderPendingPayment = 4,
    OrderPendingPaymentFailed = 5,
    PaymentSucceeded = 6,
    PaymentFailed = 7,
    OrderFailed = 8,
    OrderMarkedAsFailedError = 9,
    OrderMarkedAsConfirmedFailed = 10,
    PaymentRefunded = 11,
    ProcessingPaymentRefund = 12,
    PaymentRefundFailed = 13,
    StockCancelFailed = 14
}

