using System.Collections.Concurrent;
using OrderSagaService.Contracts.Commands.Order;
using OrderSagaService.Contracts.Commands.Payment;
using OrderSagaService.Contracts.Commands.Stock;
using OrderSagaService.Contracts.Responses.Order;
using OrderSagaService.Contracts.Responses.Payment;
using OrderSagaService.Contracts.Responses.Stock;
using OrderSagaService.Exchanges;
using OrderSagaService.Interfaces;
using OrderSagaService.State;


namespace OrderSagaService.Sagas;

public class PlaceOrderSaga
{
    private static ConcurrentDictionary<Guid, OrderSagaState> _sagaState = new();
    private readonly IMessageClient _messageClient;
    private const int MaxRetries = 3;

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
            
            case OrderSetToPendingPayment e:
                await Handle(e);
                break;
            
            case OrderSetToPendingPaymentFailed e:
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

            case StockCancelled e:
                await Handle(e);
                break;
            
            case StockCancelledFailed e:
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

        await _messageClient.PublishAsync(sellStockEvent, StockEvent.SellStockEvent);
    }

    private async Task Handle(StockSold evt)
    {
        Console.WriteLine($"StockUpdated Handle method triggered for {evt.OrderId}");
        if (!_sagaState.TryGetValue(evt.OrderId, out var saga))
        {
            Console.WriteLine($"Saga: Unknown delete request {evt.OrderId}");
        }

        saga.Action = ActionType.StockSold;

        var setOrderToPendingPaymentEvent = new SetOrderToPendingPaymentEvent
        {
            OrderId = evt.OrderId,
        };
        
        await _messageClient.PublishAsync(setOrderToPendingPaymentEvent, OrderEvent.SetOrderToPendingPaymentEvent);
    }
    
    private async Task Handle(StockSoldFailed evt)
    {
        Console.WriteLine($"StockUpdateFailed Handle method triggered for {evt.OrderId}");
        
        if (!_sagaState.TryGetValue(evt.OrderId, out var saga))
        {
            Console.WriteLine($"Saga: Unknown delete request {evt.OrderId}");
        }
        
        saga.Action =  ActionType.StockSoldFailed;

        var failOrderEvent = new PlaceOrderFailedEvent
        {
            OrderId = evt.OrderId,
            PaymentFail = false,
            StockFail = true
        };
        
        Console.WriteLine($"Trigger compensation for OrderService.");
        
        await _messageClient.PublishAsync(failOrderEvent, OrderEvent.PlaceOrderFailedEvent);
    }
    
    private async Task Handle(OrderSetToPendingPayment evt)
    {
        Console.WriteLine($"OrderSetToPendingPayment Handle method triggered for {evt.OrderId}");
        
        if (!_sagaState.TryGetValue(evt.OrderId, out var saga))
        {
            Console.WriteLine($"Saga: Unknown delete request {evt.OrderId}");
        }

        saga.Action = ActionType.OrderPendingPayment;
        
        var paymentRequestEvent = new PaymentRequestEvent
        {
            OrderId = evt.OrderId,
            Amount = saga.Price,
            UserId = saga.UserId
        };
        
        await _messageClient.PublishAsync(paymentRequestEvent, PaymentEvent.PaymentRequestEvent);
    }
    
    private async Task Handle(OrderSetToPendingPaymentFailed evt)
    {
        Console.WriteLine($"OrderSetToPendingPaymentFailed Handle method triggered for {evt.OrderId}");
        
        if (!_sagaState.TryGetValue(evt.OrderId, out var saga))
        {
            Console.WriteLine($"Saga: Unknown delete request {evt.OrderId}");
        }
        
        saga.Action = ActionType.OrderPendingPaymentFailed;
        
        var cancelStockEvent = new CancelStockEvent
        {
            OrderId =  evt.OrderId,
            ProductsAndQuantities = saga.ProductsAndQuantities
        };
        
        Console.WriteLine($"Trigger compensation for StockService.");
        await _messageClient.PublishAsync(cancelStockEvent, StockEvent.CancelStockEvent);
    }

    
    private async Task Handle(PaymentSucceeded evt)
    {
        Console.WriteLine($"PaymentSucceeded Handle method triggered for {evt.OrderId}");
        
        if (!_sagaState.TryGetValue(evt.OrderId, out var saga))
        {
            Console.WriteLine($"Saga: Unknown delete request {evt.OrderId}");
        }
        
        saga.Action =  ActionType.PaymentSucceeded;
        
        var confirmOrderEvent = new ConfirmOrderEvent
        {
            OrderId = evt.OrderId,
        };
        await _messageClient.PublishAsync(confirmOrderEvent, OrderEvent.ConfirmOrderEvent);
    }
    
    private async Task Handle(PaymentFailed evt)
    {
        Console.WriteLine($"PaymentFailed Handle method triggered for {evt.OrderId}");
        
        if (!_sagaState.TryGetValue(evt.OrderId, out var saga))
        {
            Console.WriteLine($"Saga: Unknown delete request {evt.OrderId}");
        }
        
        saga.Action = ActionType.PaymentFailed;
        
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
        
        Console.WriteLine($"Trigger compensation for Stock- and OrderService.");

        await _messageClient.PublishAsync(cancelStockEvent, StockEvent.CancelStockEvent);
        await _messageClient.PublishAsync(failOrderEvent,  OrderEvent.PlaceOrderFailedEvent);
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
            saga.Action = ActionType.OrderMarkedAsFailedError;
    }

    private async Task Handle(OrderConfirmed evt)
    {
        Console.WriteLine($"OrderConfirmed Handle method triggered for {evt.OrderId}");
        
        if (!_sagaState.TryGetValue(evt.OrderId, out var saga))
        {
            Console.WriteLine($"Saga: Unknown delete request {evt.OrderId}");
        }
        
        saga.Action = ActionType.OrderConfirmed;
        Console.WriteLine($"Workflow for {saga.OrderId} has been completed");
    }
    
    private async Task Handle(OrderConfirmFailed evt)
    {
        Console.WriteLine($"OrderConfirmFailed Handle method triggered for {evt.OrderId}");

        if (!_sagaState.TryGetValue(evt.OrderId, out var saga))
        {
            Console.WriteLine($"Saga: Unknown delete request {evt.OrderId}");
            return;
        }

        saga.Action = ActionType.OrderMarkedAsConfirmedFailed;
        saga.ConfirmRetryCount++;

        if (saga.ConfirmRetryCount <= MaxRetries)
        {
            Console.WriteLine($"Retrying order confirmation ({saga.ConfirmRetryCount}/{MaxRetries}) for {evt.OrderId}");
            var confirmOrderEvent = new ConfirmOrderEvent
            {
                OrderId = evt.OrderId
            };
            await _messageClient.PublishAsync(confirmOrderEvent, OrderEvent.ConfirmOrderEvent);
        }
        else
        {
            Console.WriteLine($"Max retries reached for order confirmation. Triggering refund for {evt.OrderId}");

            var cancelStockEvent = new CancelStockEvent
            {
                OrderId = evt.OrderId,
                ProductsAndQuantities = saga.ProductsAndQuantities
            };

            var paymentRefundEvent = new PaymentRefundEvent
            {
                OrderId = evt.OrderId,
                UserId = saga.UserId,
                Amount = saga.Price
            };

            await _messageClient.PublishAsync(cancelStockEvent, StockEvent.CancelStockEvent);
            await _messageClient.PublishAsync(paymentRefundEvent, PaymentEvent.PaymentRefundEvent);

            saga.Action = ActionType.ProcessingPaymentRefund;
        }
    }
    
    private async Task Handle(StockCancelled evt)
    {
        Console.WriteLine($"StockCancelled Handle method triggered for {evt.OrderId}");
        if (!_sagaState.TryGetValue(evt.OrderId, out var saga))
        {
            Console.WriteLine($"Saga: Unknown delete request {evt.OrderId}");
            return;
        }

        saga.Action = ActionType.OrderFailed;
        Console.WriteLine($"Stock cancellation succeeded for {saga.OrderId}");
    }

    private async Task Handle(StockCancelledFailed evt)
    {
        Console.WriteLine($"StockCancelledFailed Handle method triggered for {evt.OrderId}");
        if (!_sagaState.TryGetValue(evt.OrderId, out var saga))
        {
            Console.WriteLine($"Saga: Unknown delete request {evt.OrderId}");
            return;
        }

        saga.StockRetryCount++;

        if (saga.StockRetryCount <= MaxRetries)
        {
            Console.WriteLine($"Retrying stock cancellation ({saga.StockRetryCount}/{MaxRetries}) for {evt.OrderId}");

            var cancelStockEvent = new CancelStockEvent
            {
                OrderId = saga.OrderId,
                ProductsAndQuantities = saga.ProductsAndQuantities
            };

            await _messageClient.PublishAsync(cancelStockEvent, StockEvent.CancelStockEvent);
        }
        else
        {
            Console.WriteLine($"Max retries reached for stock cancellation. Manual intervention required for {evt.OrderId}");
            saga.Action = ActionType.StockCancelFailed;
        }
    }

    
    private async Task Handle(PaymentRefunded evt)
    {
        Console.WriteLine($"PaymentRefunded Handle method triggered for {evt.OrderId}");
        
        if (!_sagaState.TryGetValue(evt.OrderId, out var saga))
        {
            Console.WriteLine($"Saga: Unknown delete request {evt.OrderId}");
        }

        saga.Action = ActionType.PaymentRefunded;
    }
    
    private async Task Handle(PaymentRefundFailed evt)
    {
        Console.WriteLine($"PaymentRefundFailed Handle method triggered for {evt.OrderId}");

        if (!_sagaState.TryGetValue(evt.OrderId, out var saga))
        {
            Console.WriteLine($"Saga: Unknown delete request {evt.OrderId}");
            return;
        }

        saga.RefundRetryCount++;

        if (saga.RefundRetryCount <= MaxRetries)
        {
            Console.WriteLine($"Retrying payment refund ({saga.RefundRetryCount}/{MaxRetries}) for {evt.OrderId}");
            var paymentRefundEvent = new PaymentRefundEvent
            {
                OrderId = evt.OrderId,
                UserId = saga.UserId,
                Amount = saga.Price
            };
            await _messageClient.PublishAsync(paymentRefundEvent, PaymentEvent.PaymentRefundEvent);
        }
        else
        {
            Console.WriteLine($"Max retries reached for refund. Manual intervention required for {evt.OrderId}");
            saga.Action = ActionType.PaymentRefundFailed;
        }
    }
}