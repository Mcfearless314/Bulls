using System.Collections.Concurrent;
using OrderSagaService.Contracts.Commands.Order;
using OrderSagaService.Contracts.Commands.Payment;
using OrderSagaService.Contracts.Commands.Stock;
using OrderSagaService.Contracts.Responses.Order;
using OrderSagaService.Contracts.Responses.Payment;
using OrderSagaService.Contracts.Responses.Stock;
using OrderSagaService.Interfaces;
using OrderSagaService.State;


namespace OrderSagaService.Sagas;

public class PlaceOrderSaga
{
    private static ConcurrentDictionary<Guid, OrderSagaState> _sagaState = new();
    private readonly IMessageClient _messageClient;

    public PlaceOrderSaga(IMessageClient messageClient)
    {
        _messageClient = messageClient;
    }

    public async Task Handle(object message)
    {
        switch (message)
        {
            case OrderPlaced e:
                await Handle(e);
                break;
            
            case StockSold e:
                await Handle(e);
                break;
            
            case StockSoldFailed e:
                await Handle(e);
                break;
            
            case PaymentSucceeded e:
                await Handle(e);
                break;
            
            case PaymentFailed e:
                await Handle(e);
                break;
            
            case OrderMarkedAsFailed e:
                await Handle(e);
                break;
            
            case OrderConfirmed e:
                await Handle(e);
                break;
            
            case OrderConfirmFailed e:
                await Handle(e);
                break;
            
            case PaymentRefunded e:
                await Handle(e);
                break;
            
            case PaymentRefundFailed e:
                await Handle(e);
                break;
        }
    }
    
    private async Task Handle(OrderPlaced evt)
    {
        Console.WriteLine($"OrderPlaced Handle method triggered for {evt.OrderId}");
        var sagaState = new OrderSagaState
        {
            SagaId = Guid.NewGuid(),
            OrderId = evt.OrderId, 
            UserId = evt.UserId,
            Action = ActionType.OrderPlaced,
            ProductsAndQuantities = evt.ProductsAndQuantities,
            Price = evt.Price,
        };
        
        _sagaState[evt.OrderId] = sagaState;

        var sellStockEvent = new SellStockEvent
        {
            OrderId =  evt.OrderId,
            ProductsAndQuantities = evt.ProductsAndQuantities
        };

        await _messageClient.PublishAsync(sellStockEvent);
    }

    private async Task Handle(StockSold evt)
    {
        Console.WriteLine($"StockUpdated Handle method triggered for {evt.OrderId}");
        if (!_sagaState.TryGetValue(evt.OrderId, out var saga))
        {
            Console.WriteLine($"Saga: Unknown delete request {evt.OrderId}");
        }

        var paymentRequestEvent = new PaymentRequestEvent
        {
            OrderId = evt.OrderId,
            Amount = saga.Price,
            UserId = saga.UserId
        };
        
        await _messageClient.PublishAsync(paymentRequestEvent);
    }
    
    private async Task Handle(StockSoldFailed evt)
    {
        Console.WriteLine($"StockUpdateFailed Handle method triggered for {evt.OrderId}");
        
        if (!_sagaState.TryGetValue(evt.OrderId, out var saga))
        {
            Console.WriteLine($"Saga: Unknown delete request {evt.OrderId}");
        }

        var failOrderEvent = new PlaceOrderFailedEvent
        {
            OrderId = evt.OrderId,
            PaymentFail = false,
            StockFail = true
        };
        
        Console.WriteLine($"Trigger compensation for OrderService.");
        
        await _messageClient.PublishAsync(failOrderEvent);
    }

    
    private async Task Handle(PaymentSucceeded evt)
    {
        Console.WriteLine($"PaymentSucceeded Handle method triggered for {evt.OrderId}");
        
        if (!_sagaState.TryGetValue(evt.OrderId, out var saga))
        {
            Console.WriteLine($"Saga: Unknown delete request {evt.OrderId}");
        }
        
        var confirmOrderEvent = new ConfirmOrderEvent
        {
            OrderId = evt.OrderId,
        };
        await _messageClient.PublishAsync(confirmOrderEvent);
    }
    
    private async Task Handle(PaymentFailed evt)
    {
        Console.WriteLine($"PaymentFailed Handle method triggered for {evt.OrderId}");
        
        if (!_sagaState.TryGetValue(evt.OrderId, out var saga))
        {
            Console.WriteLine($"Saga: Unknown delete request {evt.OrderId}");
        }
        
        var cancelStockEvent = new CancelStockEvent
        {
            OrderId =  evt.OrderId,
            ProductsAndQuantities = saga.ProductsAndQuantities
        };

        var failOrderEvent = new PlaceOrderFailedEvent
        {
            OrderId = evt.OrderId,
            PaymentFail = true,
            StockFail = false
        };
        
        saga.Action = ActionType.PaymentFailed;
        
        Console.WriteLine($"Trigger compensation for Stock- and OrderService.");

        await _messageClient.PublishAsync(cancelStockEvent);
        await _messageClient.PublishAsync(failOrderEvent);
    }

    private async Task Handle(OrderMarkedAsFailed evt)
    {
        Console.WriteLine($"OrderMarkedAsFailed Handle method triggered for {evt.OrderId}");
        
        if (!_sagaState.TryGetValue(evt.OrderId, out var saga))
        {
            Console.WriteLine($"Saga: Unknown delete request {evt.OrderId}");
        }

        if (evt.Success)
            saga.Action = ActionType.OrderFailed;
        else
            saga.Action = ActionType.OrderError;
    }

    private async Task Handle(OrderConfirmed evt)
    {
        Console.WriteLine($"OrderConfirmed Handle method triggered for {evt.OrderId}");
        
        if (!_sagaState.TryGetValue(evt.OrderId, out var saga))
        {
            Console.WriteLine($"Saga: Unknown delete request {evt.OrderId}");
        }
        
        saga.Action = ActionType.OrderPlaced;
        Console.WriteLine($"Workflow for {saga.OrderId} has been completed");
    }
    
    private async Task Handle(OrderConfirmFailed evt)
    {
        Console.WriteLine($"OrderConfirmFailed Handle method triggered for {evt.OrderId}");
        
        if (!_sagaState.TryGetValue(evt.OrderId, out var saga))
        {
            Console.WriteLine($"Saga: Unknown delete request {evt.OrderId}");
        }

        saga.Action = ActionType.OrderFailed;
        
        var cancelStockEvent = new CancelStockEvent
        {
            OrderId =  evt.OrderId,
            ProductsAndQuantities = saga.ProductsAndQuantities
        };

        var paymentRefundEvent = new PaymentRefundEvent
        {
            OrderId = evt.OrderId,
            UserId = saga.UserId,
            Amount = saga.Price
        };

        Console.WriteLine($"Trigger compensation for Stock- and PaymentService.");
        
        await _messageClient.PublishAsync(cancelStockEvent);
        await _messageClient.PublishAsync(paymentRefundEvent);
        
        Console.WriteLine($"Workflow for {saga.OrderId} has failed.");
    }
    
    private async Task Handle(PaymentRefunded evt)
    {
        Console.WriteLine($"PaymentRefunded Handle method triggered for {evt.OrderId}");
        
        if (!_sagaState.TryGetValue(evt.OrderId, out var saga))
        {
            Console.WriteLine($"Saga: Unknown delete request {evt.OrderId}");
        }

        saga.Action = ActionType.PaymentRefundedForFailedOrder;
    }
    
    private async Task Handle(PaymentRefundFailed evt)
    {
        Console.WriteLine($"PaymentRefundFailed Handle method triggered for {evt.OrderId}");
        
        if (!_sagaState.TryGetValue(evt.OrderId, out var saga))
        {
            Console.WriteLine($"Saga: Unknown delete request {evt.OrderId}");
        }

        saga.Action = ActionType.RefundErrorForFailedOrder;
    }
}