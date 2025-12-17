using OrderSagaService.Contracts.Responses.Order;
using OrderSagaService.Contracts.Responses.Payment;
using OrderSagaService.Contracts.Responses.Stock;
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
        await _messageClient.SubscribeTestAsync<OrderPlaced>(
            "place_order_subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken, "order-placed"
        );
        
        await _messageClient.SubscribeAsync<OrderPlacingFailed>(
            "order_placing_failed_subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken
        );

        await _messageClient.SubscribeAsync<StockSold>(
            "update_stock_subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken
        );
        
        await _messageClient.SubscribeAsync<StockSoldFailed>(
            "update_failed_stock_subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken
        );
        
        await _messageClient.SubscribeAsync<PaymentSucceeded>(
            "payment_succeeded_subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken
        );
        
        await _messageClient.SubscribeAsync<PaymentFailed>(
            "payment_failed_subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken
        );
        
        await _messageClient.SubscribeAsync<OrderMarkedAsFailed>(
            "confirm_order_marked_as_failed_subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken
        );
        
        await _messageClient.SubscribeAsync<OrderConfirmed>(
            "confirm_order_subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken
        );
        
        await _messageClient.SubscribeAsync<OrderConfirmFailed>(
            "order_confirm_failed_subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken
        );
        
        await _messageClient.SubscribeAsync<PaymentRefunded>(
            "payment_refunded_subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken
        );
        
        await _messageClient.SubscribeAsync<PaymentRefundFailed>(
            "payment_refunded_failed_subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken
        );

        Console.WriteLine("Saga subscriptions active.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}