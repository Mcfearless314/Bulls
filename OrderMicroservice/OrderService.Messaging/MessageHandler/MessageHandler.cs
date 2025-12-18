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
        await _messageClient.SubscribeAsync<AddOrderItemToOrder>("add-order-item-to-order", AddOrderItemToOrder,
            cancellationToken, OrderEvent.AddOrderItemToOrderEvent);

        await _messageClient.SubscribeAsync<AddingOrderItemToOrderFailedEvent>("add-order-item-to-order-failed",
            AddOrderItemToOrderFailed, cancellationToken, OrderEvent.AddOrderItemToOrderFailedEvent);

        await _messageClient.SubscribeAsync<PlaceOrderFailedEvent>("place-order-failed", PlaceOrderFailed,
            cancellationToken, OrderEvent.PlaceOrderFailedEvent);

        await _messageClient.SubscribeAsync<ConfirmOrderEvent>("confirm-order", ConfirmOrder,
            cancellationToken, OrderEvent.ConfirmOrderEvent);
        
        await _messageClient.SubscribeAsync<SetOrderToPendingPaymentEvent>("set-order-to-pending-payment", SetOrderToPendingPayment, 
            cancellationToken, OrderEvent.SetOrderToPendingPaymentEvent);
    }

    #region AddOrderItemToOrder

    private Task AddOrderItemToOrder(AddOrderItemToOrder arg)
    {
        Console.WriteLine("AddOrderItemToOrder triggered");
        return _orderService.AddItemToOrderTask(arg.OrderId, arg.ProductId, arg.ProductName, arg.Price, arg.Quantity);
    }

    #endregion

    #region AddOrderItemToOrderFailed

    private Task AddOrderItemToOrderFailed(AddingOrderItemToOrderFailedEvent arg)
    {
        Console.WriteLine("AddOrderItemToOrderFailed triggered");
        Console.WriteLine($"Could not add item to order. OrderId: " + arg.OrderId + ", ProductId: " + arg.ProductId +
                          ", Reason: " + arg.Reason);
        return Task.CompletedTask;
    }

    #endregion

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
            
            var orderMarkedAsFailed = new OrderMarkedAsFailed
            {
                OrderId = arg.OrderId,
                Success = true
            };
            await _messageClient.PublishAsync(orderMarkedAsFailed, OrderEvent.OrderMarkedAsFailed);
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
    }

    #endregion

    #region ConfirmOrder

    private async Task ConfirmOrder(ConfirmOrderEvent arg)
    {
        Console.WriteLine("ConfirmOrder triggered");
        try
        {
            await _orderService.UpdateOrderStatus(arg.OrderId, OrderStatus.Confirmed);
            
            var orderConfirmed = new OrderConfirmed
            {
                OrderId = arg.OrderId
            };
            await _messageClient.PublishAsync(orderConfirmed, OrderEvent.OrderConfirmed);
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
    }

    #endregion

    #region SetOrderToPendingPayment

    private async Task SetOrderToPendingPayment(SetOrderToPendingPaymentEvent arg)
    {
        Console.WriteLine("SetOrderToPendingPayment triggered");
        try
        {
            await _orderService.UpdateOrderStatus(arg.OrderId, OrderStatus.PendingPayment);
            
            var orderConfirmed = new OrderSetToPendingPayment
            {
                OrderId = arg.OrderId
            };
            Console.WriteLine($"Publishing {OrderEvent.SetOrderToPendingPaymentEvent}");
            await _messageClient.PublishAsync(orderConfirmed, OrderEvent.OrderSetToPendingPayment);
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
    }

    #endregion
}