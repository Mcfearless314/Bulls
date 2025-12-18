using OrderSagaService.Contracts.Responses.Order;
using OrderSagaService.Contracts.Responses.Payment;
using OrderSagaService.Contracts.Responses.Stock;
using OrderSagaService.Exchanges;
using OrderSagaService.Interfaces;
using OrderSagaService.Sagas;

namespace OrderSagaService.Workers;

public class PlaceOrderSagaBackgroundService : BackgroundService
{
    private readonly IMessageClient _messageClient;
    private readonly PlaceOrderSaga _placeOrderSaga;

    public PlaceOrderSagaBackgroundService(IMessageClient messageClient, PlaceOrderSaga placeOrderSaga)
    {
        _messageClient = messageClient;
        _placeOrderSaga = placeOrderSaga;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _messageClient.SubscribeAsync<OrderPlaced>(
            "place_order_subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken, OrderEvent.OrderPlaced
        );
        
        await _messageClient.SubscribeAsync<OrderPlacingFailed>(
            "order_placing_failed_subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken, OrderEvent.OrderPlacingFailed
        );

        await _messageClient.SubscribeAsync<StockSold>(
            "update_stock_subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken, StockEvent.StockSold
        );
        
        await _messageClient.SubscribeAsync<StockSoldFailed>(
            "update_failed_stock_subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken, StockEvent.StockSoldFailed
        );
        
        await _messageClient.SubscribeAsync<PaymentSucceeded>(
            "payment_succeeded_subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken, PaymentEvent.PaymentSucceeded
        );
        
        await _messageClient.SubscribeAsync<PaymentFailed>(
            "payment_failed_subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken, PaymentEvent.PaymentFailed
        );
        
        await _messageClient.SubscribeAsync<OrderSetToPendingPayment>(
            "order_set_to_pending_payment_subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken, OrderEvent.OrderSetToPendingPayment
        );
        
        await _messageClient.SubscribeAsync<OrderSetToPendingPaymentFailed>(
            "order_set_to_pending_payment_failed_subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken, OrderEvent.OrderSetToPendingPaymentFailed
        );
        
        
        await _messageClient.SubscribeAsync<OrderMarkedAsFailed>(
            "confirm_order_marked_as_failed_subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken,  OrderEvent.OrderMarkedAsFailed
        );
        
        await _messageClient.SubscribeAsync<OrderConfirmed>(
            "confirm_order_subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken, OrderEvent.OrderConfirmed
        );
        
        await _messageClient.SubscribeAsync<OrderConfirmFailed>(
            "order_confirm_failed_subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken, OrderEvent.OrderConfirmFailed
        );
        
        await _messageClient.SubscribeAsync<StockCancelled>(
            "stock_cancelled_subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken, StockEvent.StockCancelled
        );
        
        await _messageClient.SubscribeAsync<StockCancelledFailed>(
            "stock_cancelled_failed_subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken, StockEvent.StockCancelledFailed
        );

        await _messageClient.SubscribeAsync<PaymentRefunded>(
            "payment_refunded_subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken, PaymentEvent.PaymentRefunded
        );
        
        await _messageClient.SubscribeAsync<PaymentRefundFailed>(
            "payment_refunded_failed_subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken, PaymentEvent.PaymentRefundFailed
        );

        Console.WriteLine("Saga subscriptions active.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}