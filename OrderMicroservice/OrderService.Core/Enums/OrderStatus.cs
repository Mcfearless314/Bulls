namespace OrderService.Core.Enums;

public enum OrderStatus
{
    PendingConfirmation = 1,
    PendingPayment = 2,
    Confirmed = 3,
    Completed = 4,
    PaymentFailed = 5,
    ErrorInStock = 6,
    CancelledByCustomer = 7,
    StockReleased = 8,
    CancelledConfirm = 9
}