namespace OrderSagaService.Exchanges;

public class StockEvent
{
    // Contracts
    public const string CancelStockEvent = "cancel_stock_event";
    public const string FreeProductReservationEvent = "free_product_reservation_event";
    public const string ReserveProductEvent = "reserve_product_event";
    public const string SellStockEvent = "sell_stock_event";
    

    // Domain Events
    public const string StockCancelled = "stock_cancelled";
    public const string StockCancelledFailed = "stock_cancelled_failed";
    public const string StockReleased = "stock_released";
    public const string StockReleaseFailed = "stock_release_failed";
    public const string StockReserved = "stock_reserved";
    public const string StockReserveFailed = "stock_reserve_failed";
    public const string StockSold = "stock_sold";
    public const string StockSoldFailed = "stock_sold_failed";
}