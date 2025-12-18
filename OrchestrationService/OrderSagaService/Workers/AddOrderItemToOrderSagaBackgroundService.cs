using OrderSagaService.Contracts.Responses.Order;
using OrderSagaService.Contracts.Responses.Stock;
using OrderSagaService.Exchanges;
using OrderSagaService.Interfaces;
using OrderSagaService.Sagas;

namespace OrderSagaService.Workers;

public class AddOrderItemToOrderSagaBackgroundService : BackgroundService
{
    private readonly IMessageClient _messageClient;
    private readonly AddOrderItemToOrderSaga _addOrderItemToOrderSaga;

    public AddOrderItemToOrderSagaBackgroundService(IMessageClient messageClient,
        AddOrderItemToOrderSaga addOrderItemToOrderSaga)
    {
        _messageClient = messageClient;
        _addOrderItemToOrderSaga = addOrderItemToOrderSaga;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _messageClient.SubscribeAsync<ProductAddedToOrder>("add_product_to_order_subscription",
            async msg => await _addOrderItemToOrderSaga.Handle(msg),
            stoppingToken, OrderEvent.ProductAddedToOrder);

        await _messageClient.SubscribeAsync<StockReserved>("stock_reserved_subscription",
            async msg => await _addOrderItemToOrderSaga.Handle(msg), stoppingToken, StockEvent.StockReserved);

        await _messageClient.SubscribeAsync<StockReserveFailed>("stock_reserve_failed_subscription",
            async msg => await _addOrderItemToOrderSaga.Handle(msg), stoppingToken, StockEvent.StockReserveFailed);
    }
}