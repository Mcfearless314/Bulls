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
            "place-order-subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken, OrderEvent.OrderPlaced
        );
        
        await _messageClient.SubscribeAsync<OrderPlacingFailed>(
            "order-placing-failed-subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken, OrderEvent.OrderPlacingFailed
        );

        await _messageClient.SubscribeAsync<StockSold>(
            "update-stock-subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken, StockEvent.StockSold
        );
        
        await _messageClient.SubscribeAsync<StockSoldFailed>(
            "update-failed-stock-subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken, StockEvent.StockSoldFailed
        );
        
        await _messageClient.SubscribeAsync<PaymentSucceeded>(
            "payment-succeeded-subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken, PaymentEvent.PaymentSucceeded
        );
        
        await _messageClient.SubscribeAsync<PaymentFailed>(
            "payment-failed-subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken, PaymentEvent.PaymentFailed
        );
        
        await _messageClient.SubscribeAsync<OrderSetToPendingPayment>(
            "order-set-to-pending-payment-subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken, OrderEvent.OrderSetToPendingPayment
        );
        
        await _messageClient.SubscribeAsync<OrderSetToPendingPaymentFailed>(
            "order-set-to-pending-payment-failed-subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken, OrderEvent.OrderSetToPendingPaymentFailed
        );
        
        
        await _messageClient.SubscribeAsync<OrderMarkedAsFailed>(
            "confirm-order-marked-as-failed-subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken,  OrderEvent.OrderMarkedAsFailed
        );
        
        await _messageClient.SubscribeAsync<OrderConfirmed>(
            "confirm-order-subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken, OrderEvent.OrderConfirmed
        );
        
        await _messageClient.SubscribeAsync<OrderConfirmFailed>(
            "order-confirm-failed-subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken, OrderEvent.OrderConfirmFailed
        );
        
        await _messageClient.SubscribeAsync<StockCancelled>(
            "stock-cancelled-subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken, StockEvent.StockCancelled
        );
        
        await _messageClient.SubscribeAsync<StockCancelledFailed>(
            "stock-cancelled-failed-subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken, StockEvent.StockCancelledFailed
        );

        await _messageClient.SubscribeAsync<PaymentRefunded>(
            "payment-refunded-subscription",
            async msg => await _placeOrderSaga.Handle(msg),
            stoppingToken, PaymentEvent.PaymentRefunded
        );
        
        await _messageClient.SubscribeAsync<PaymentRefundFailed>(
            "payment-refunded-failed-subscription",
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