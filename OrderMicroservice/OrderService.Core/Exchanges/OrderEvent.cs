namespace OrderService.Core.Exchanges;

public class OrderEvent
{
    // Contracts
    public const string CancelOrderEvent = "cancel_order_event";
    public const string ConfirmOrderEvent = "confirm_order_event";
    public const string PlaceOrderEvent = "place_order_event";
    public const string PlaceOrderFailedEvent = "place_order_failed_event";
    public const string SetOrderToPendingPaymentEvent = "set_order_to_pending_payment_event";

    // Domain events
    public const string OrderPlaced = "order_placed";
    public const string OrderPlacingFailed = "order_placing_failed";
    public const string OrderCancelled = "order_cancelled";
    public const string OrderCancellationFailed = "order_cancellation_failed";
    public const string OrderConfirmed = "order_confirmed";
    public const string OrderConfirmFailed = "order_confirmed_failed";
    public const string OrderMarkedAsFailed = "order_marked_as_failed";
    public const string OrderSetToPendingPayment = "order_set_to_pending_payment";
    public const string OrderSetToPendingPaymentFailed = "order_set_to_pending_payment_failed";
    public const string ProductAddedToOrder = "product_added_to_order";
    public const string ProductAddedToOrderFailed = "product_added_to_order_failed";
    public const string RemovedProductFromOrderItems = "removed_product_from_order_items";  
    public const string RemovalOfProductFromOrderItemsFailed = "removal_of_product_from_order_items_failed";  
}