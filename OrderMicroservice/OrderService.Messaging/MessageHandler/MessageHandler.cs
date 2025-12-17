using OrderService.Core.Interfaces;
using OrderService.Core.Contracts;
using OrderService.Core.DomainEvents;
using OrderService.Core.Enums;

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
       
        await _messageClient.SubscribeAsync<PlaceOrderFailedEvent>("place-order-failed", PlaceOrderFailed, cancellationToken);
        
        await _messageClient.SubscribeAsync<ConfirmOrderEvent>("place-order-failed", ConfirmOrder, cancellationToken);
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
            await _messageClient.PublishAsync(orderMarkedAsFailed);
        }
        finally
        {
            var orderMarkedAsFailed = new OrderMarkedAsFailed
            {
                OrderId = arg.OrderId,
                Success = true
            };
            await _messageClient.PublishAsync(orderMarkedAsFailed);
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
            await _messageClient.PublishAsync(confirmOrderFailed);
        }
        finally
        {
            var orderConfirmed = new OrderConfirmed
            {
                OrderId = arg.OrderId
            };
            await _messageClient.PublishAsync(orderConfirmed);
        }
    }
    
    #endregion 
    
}