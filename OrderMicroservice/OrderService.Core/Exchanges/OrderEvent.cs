namespace OrderService.Core.Exchanges;

public class OrderEvent
{
    public const string OrderPlaced = "order_placed";
    public const string OrderPlacingFailed = "order_placing_failed";
    public const string OrderCancelled = "order_cancelled";
    public const string OrderCancellationFailed = "order_cancellation_failed";
    public const string OrderConfirmed = "order_confirmed";
    public const string OrderConfirmedFailed = "order_confirmed_failed";
    public const string OrderMarkedAsFailed = "order_marked_as_failed";
    public const string ProductAddedToOrder = "product_added_to_order";
    public const string ProductAddedToOrderFailed = "product_added_to_order_failed";   
    public const string RemovalOfProductFromOrderItemsFailed = "removal_of_product_from_order_items_failed";  
}