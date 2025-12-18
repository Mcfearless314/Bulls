using OrderService.Core.Interfaces;
using OrderService.Core.Contracts;
using OrderService.Core.DomainEvents;
using OrderService.Core.Enums;
using OrderService.Core.Exchanges;

namespace OrderService.Messaging.MessageHandler;

public class MessageHandler : IMessageHandler
{
    private readonly IMessageClient _messageClient;
    private readonly Application.Services.OrderService _orderService;

    public MessageHandler(IMessageClient messageClient, Application.Services.OrderService orderService)
    {
        _messageClient = messageClient;
        _orderService = orderService;
    }

    public async Task Subscribe(CancellationToken cancellationToken)
    {
       
        await _messageClient.SubscribeAsync<PlaceOrderFailedEvent>("place-order-failed", PlaceOrderFailed, 
            cancellationToken, OrderEvent.PlaceOrderEvent);
        
        await _messageClient.SubscribeAsync<ConfirmOrderEvent>("place-order-failed", ConfirmOrder, 
            cancellationToken, OrderEvent.ConfirmOrderEvent);
        
        await _messageClient.SubscribeAsync<SetOrderToPendingPaymentEvent>("set-order-to-pending_payment", SetOrderToPendingPayment, 
            cancellationToken, OrderEvent.SetOrderToPendingPaymentEvent);
    }

    #region PlaceOrderFailed
    
    private async Task PlaceOrderFailed(PlaceOrderFailedEvent arg)
    {
        Console.WriteLine("PlaceOrderFailed triggered");
        try
        {
            if (arg.PaymentFail)
                await _orderService.UpdateOrderStatus(arg.OrderId, OrderStatus.PaymentFailed);
            if (arg.StockFail)
                await _orderService.UpdateOrderStatus(arg.OrderId, OrderStatus.ErrorInStock);
        }
        catch (Exception ex)
        {
            var orderMarkedAsFailed = new OrderMarkedAsFailed
            {
                OrderId = arg.OrderId,
                Success = false
            };
            await _messageClient.PublishAsync(orderMarkedAsFailed, OrderEvent.OrderMarkedAsFailed);
        }
        finally
        {
            var orderMarkedAsFailed = new OrderMarkedAsFailed
            {
                OrderId = arg.OrderId,
                Success = true
            };
            await _messageClient.PublishAsync(orderMarkedAsFailed, OrderEvent.OrderMarkedAsFailed);
        }

    }

    #endregion
    
    #region ConfirmOrder
    
    private async Task ConfirmOrder(ConfirmOrderEvent arg)
    {
        Console.WriteLine("ConfirmOrder triggered");
        try
        {
            await _orderService.UpdateOrderStatus(arg.OrderId, OrderStatus.Confirmed);
        }
        catch (Exception ex)
        {
            var confirmOrderFailed = new OrderConfirmFailed
            {
                OrderId = arg.OrderId,
                Reason = ex.Message,
            };
            await _messageClient.PublishAsync(confirmOrderFailed, OrderEvent.OrderConfirmFailed);
        }
        finally
        {
            var orderConfirmed = new OrderConfirmed
            {
                OrderId = arg.OrderId
            };
            await _messageClient.PublishAsync(orderConfirmed, OrderEvent.OrderConfirmed);
        }
    }
    
    #endregion 
    
    #region SetOrderToPendingPayment
    
    private async Task SetOrderToPendingPayment(SetOrderToPendingPaymentEvent arg)
    {
        Console.WriteLine("SetOrderToPendingPayment triggered");
        try
        {
            await _orderService.UpdateOrderStatus(arg.OrderId, OrderStatus.PendingPayment);
        }
        catch (Exception ex)
        {
            var confirmOrderFailed = new OrderSetToPendingPaymentFailed
            {
                OrderId = arg.OrderId,
                Reason = ex.Message,
            };
            await _messageClient.PublishAsync(confirmOrderFailed, OrderEvent.OrderSetToPendingPaymentFailed);
        }
        finally
        {
            var orderConfirmed = new OrderSetToPendingPayment
            {
                OrderId = arg.OrderId
            };
            await _messageClient.PublishAsync(orderConfirmed, OrderEvent.OrderSetToPendingPayment);
        }
    }
    
    #endregion
    
}