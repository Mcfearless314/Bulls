namespace OrderSagaService.State;

public class OrderSagaState
{
    public Guid SagaId { get; set; }
    public Guid OrderId { get; set; }
    public int UserId { get; set; }
    public ActionType Action { get; set; }
    public Dictionary<int, int> ProductsAndQuantities { get; set; }
    public double Price { get; set; }
}

public enum ActionType
{
    OrderPlaced = 1,
    OrderCancelled = 2,
    OrderFailed = 3,
    ReserveStock = 4,
    ProcessPayment = 5,
    CompleteOrder = 6,
    CancelStock = 7,
    PaymentRefundedForFailedOrder = 8,
    RefundErrorForFailedOrder = 9,
    PaymentFailed = 10,
    OrderError = 11
}

