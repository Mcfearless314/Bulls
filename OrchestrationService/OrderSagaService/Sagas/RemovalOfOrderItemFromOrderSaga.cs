using System.Collections.Concurrent;
using OrderSagaService.Contracts.Responses.Order;
using OrderSagaService.Contracts.Responses.Stock;
using OrderSagaService.Exchanges;
using OrderSagaService.Interfaces;
using OrderSagaService.State;

namespace OrderSagaService.Sagas;

public class RemovalOfOrderItemFromOrderSaga
{
    private static ConcurrentDictionary<Guid, RemovalOfOrderItemFromOrderSagaState> _sagaState = new();
    private readonly IMessageClient _messageClient;
    private const int MaxRetries = 3;
    
    public RemovalOfOrderItemFromOrderSaga(IMessageClient messageClient)
    {
        _messageClient = messageClient;
    }

    public async Task Handle(object message)
    {
        switch (message)
        {
            case RemovedProductFromOrderItems eventMessage:
                await HandleRemovedProductFromOrderItems(eventMessage);
                break;
            case StockReleased eventMessage:
                HandleStockReleased(eventMessage);
                break;
            case StockReleasedFailed eventMessage:
                await HandleStockReleasedFailed(eventMessage);
                break;
            default:
                throw new InvalidOperationException("Unknown message type");
        }
    }

    private async Task HandleRemovedProductFromOrderItems(RemovedProductFromOrderItems eventMessage)
    {
        Console.WriteLine($"Removed Product From Order with orderId: {eventMessage.OrderId}, productId: {eventMessage.ProductId}, quantity: {eventMessage.Quantity}");
        var sagaState = new RemovalOfOrderItemFromOrderSagaState
        {
            SagaId = Guid.NewGuid(),
            OrderId = eventMessage.OrderId,
            ProductId = eventMessage.ProductId,
            Quantity = eventMessage.Quantity
        };
        
        _sagaState[eventMessage.OrderId] = sagaState;
        
        var freeProductReservationEvent = new Contracts.Commands.Stock.FreeProductReservationEvent
        {
            ProductId = eventMessage.ProductId,
            Quantity = eventMessage.Quantity,
            OrderId = eventMessage.OrderId
        };

        await _messageClient.PublishAsync(freeProductReservationEvent, StockEvent.FreeProductReservationEvent);
    }

    private void HandleStockReleased(StockReleased eventMessage)
    {
        Console.WriteLine("Stock released successfully for orderId: " + eventMessage.OrderId);
    }

    private async Task HandleStockReleasedFailed(StockReleasedFailed eventMessage)
    {
        Console.WriteLine($"Stock release failed for orderId: {eventMessage.OrderId} with reason: {eventMessage.Reason}");
        if (! _sagaState.TryGetValue(eventMessage.OrderId, out var sagaState))
        {
            Console.WriteLine($"Saga: No saga state found for OrderId: {eventMessage.OrderId}");
            return; 
        }

        sagaState.StockRetryCount++;

        if (sagaState.StockRetryCount >= MaxRetries)
        {
            var freeProductReservationEvent = new Contracts.Commands.Stock.FreeProductReservationEvent
            {
                ProductId = sagaState.ProductId,
                Quantity = sagaState.Quantity,
                OrderId = eventMessage.OrderId
            };

            await _messageClient.PublishAsync(freeProductReservationEvent, StockEvent.FreeProductReservationEvent);
        }
        else 
        {
           Console.WriteLine($"Saga: Max retries reached for stock release for OrderId: {eventMessage.OrderId}. Final reason for failure: {eventMessage.Reason}. No further attempts will be made.");
        }
    }
}