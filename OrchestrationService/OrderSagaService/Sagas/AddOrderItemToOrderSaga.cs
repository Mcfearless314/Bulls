using System.Collections.Concurrent;
using OrderSagaService.Contracts.Commands.Order;
using OrderSagaService.Contracts.Commands.Stock;
using OrderSagaService.Contracts.Responses.Order;
using OrderSagaService.Contracts.Responses.Stock;
using OrderSagaService.Exchanges;
using OrderSagaService.Interfaces;
using OrderSagaService.State;

namespace OrderSagaService.Sagas;

public class AddOrderItemToOrderSaga
{
    private static ConcurrentDictionary<Guid, AddOrderItemToOrderSagaState> _sagaState = new();
    private readonly IMessageClient _messageClient;
    private const int MaxRetries = 3;
    
    public AddOrderItemToOrderSaga(IMessageClient messageClient)
    {
        _messageClient = messageClient;
    }

    public async Task Handle(object message)
    {
        switch (message)
        {
            case ProductAddedToOrder eventMessage:
                await HandleProductAddedToOrder(eventMessage);
                break;
            case StockReserved eventMessage:
                await HandleStockReserved(eventMessage);
                break;
            case StockReserveFailed eventMessage:
                await HandleStockReserveFailed(eventMessage);
                break;
            default:
                Console.WriteLine($"AddOrderItemToOrderSaga: Unknown message type received.");
                break;
        }
    }
    
    private async Task HandleProductAddedToOrder(ProductAddedToOrder message)
    {
        Console.WriteLine($"Product addition request for product {message.ProductId} received. Adding to order {message.OrderId}. Quantity: {message.Quantity}");
        var sagaState = new AddOrderItemToOrderSagaState
        {
            SagaId = Guid.NewGuid(),
            OrderId = message.OrderId,
            ProductId = message.ProductId,
            Quantity = message.Quantity,
        };
        
        _sagaState[message.OrderId] = sagaState;

        var reserveProductEvent = new ReserveProductEvent
        {
            ProductId = message.ProductId,
            Quantity = message.Quantity,
            OrderId = message.OrderId
        };
        
        await _messageClient.PublishAsync(reserveProductEvent, StockEvent.ReserveProductEvent);
    }
    
    private async Task HandleStockReserved(StockReserved eventMessage)
    {
        Console.WriteLine($"Stock reserved for product {eventMessage.ProductId} for order {eventMessage.OrderId}. Quantity: {eventMessage.Quantity}");
        if (! _sagaState.TryGetValue(eventMessage.OrderId, out var sagaState))
        {
            Console.WriteLine($"Saga: No saga state found for OrderId: {eventMessage.OrderId}");
            return; 
        }

        sagaState.ProductName = eventMessage.ProductName;
        sagaState.Price = eventMessage.Price;
        
        await _messageClient.PublishAsync(new AddOrderItemToOrderEvent
        {
            ProductId = eventMessage.ProductId,
            Quantity = eventMessage.Quantity,
            ProductName = eventMessage.ProductName,
            Price = eventMessage.Price,
            OrderId = eventMessage.OrderId,
            
        }, OrderEvent.AddOrderItemToOrderEvent);
    }
    
    private async Task HandleStockReserveFailed(StockReserveFailed eventMessage)
    {
        Console.WriteLine($"Stock reservation failed for product {eventMessage.ProductId} for order {eventMessage.OrderId}. Reason: {eventMessage.Reason}");
        if (! _sagaState.TryGetValue(eventMessage.OrderId, out var sagaState))
        {
            Console.WriteLine($"Saga: No saga state found for OrderId: {eventMessage.OrderId}");
            return; 
        }

        sagaState.StockRetryCount++;

        if (sagaState.StockRetryCount <= MaxRetries)
        {
            Console.WriteLine($"Retrying stock reservation for product {eventMessage.ProductId} for order {eventMessage.OrderId}. Attempt {sagaState.StockRetryCount}");
            var reserveProductEvent = new ReserveProductEvent
            {
                ProductId = eventMessage.ProductId,
                Quantity = eventMessage.Quantity,
                OrderId = eventMessage.OrderId
            };
            
            await _messageClient.PublishAsync(reserveProductEvent, StockEvent.ReserveProductEvent);
        }
        else
        {
            Console.WriteLine($"Max retries reached for stock reservation for product {eventMessage.ProductId} for order {eventMessage.OrderId}. Failing saga.");
            var reserveProductFailedEvent = new AddOrderItemToOrderFailedEvent
            {
                OrderId = eventMessage.OrderId,
                ProductId = eventMessage.ProductId,
                ProductName = eventMessage.ProductName,
                Price = eventMessage.Price,
                Quantity = eventMessage.Quantity,
                Reason = eventMessage.Reason
            };
            
            await _messageClient.PublishAsync(reserveProductFailedEvent, OrderEvent.AddOrderItemToOrderFailedEvent);
        }
    }
    
    
}