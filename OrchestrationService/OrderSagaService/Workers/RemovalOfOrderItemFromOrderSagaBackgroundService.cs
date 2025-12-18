using OrderSagaService.Contracts.Responses.Order;
using OrderSagaService.Contracts.Responses.Stock;
using OrderSagaService.Exchanges;
using OrderSagaService.Interfaces;
using OrderSagaService.Sagas;

namespace OrderSagaService.Workers;

public class RemovalOfOrderItemFromOrderSagaBackgroundService : BackgroundService
{
    private readonly IMessageClient _messageClient;
    private readonly RemovalOfOrderItemFromOrderSaga  _removalOfOrderItemFromOrderSaga;
    
    public RemovalOfOrderItemFromOrderSagaBackgroundService(IMessageClient messageClient,
        RemovalOfOrderItemFromOrderSaga removalOfOrderItemFromOrderSaga)
    {
        _messageClient = messageClient;
        _removalOfOrderItemFromOrderSaga = removalOfOrderItemFromOrderSaga;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _messageClient.SubscribeAsync<RemovedProductFromOrderItems>("remove_product_from_order_subscription",
            async msg => await _removalOfOrderItemFromOrderSaga.Handle(msg),
            stoppingToken, OrderEvent.RemovedProductFromOrderItems);

        await _messageClient.SubscribeAsync<StockReleased>("stock_released_subscription",
            async msg => await _removalOfOrderItemFromOrderSaga.Handle(msg),
            stoppingToken, StockEvent.StockReleased);
        
        await _messageClient.SubscribeAsync<StockReleasedFailed>("stock_release_failed_subscription",
            async msg => await _removalOfOrderItemFromOrderSaga.Handle(msg),
            stoppingToken, StockEvent.StockReleaseFailed);
    }
}