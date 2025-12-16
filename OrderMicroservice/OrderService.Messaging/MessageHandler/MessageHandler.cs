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
        await _messageClient.SubscribeAsync<StockUpdated>("stock-updated", StockUpdated, cancellationToken);

        await _messageClient.SubscribeAsync<PaymentSucceeded>("payment-succeeded", PaymentSucceeded, cancellationToken);

        await _messageClient.SubscribeAsync<PaymentRefunded>("payment-refunded", PaymentRefunded, cancellationToken);
    }

    #region StockUpdated

    private async Task StockUpdated(StockUpdated arg)
    {
        if (arg.IsSold.HasValue && arg.IsSold.Value)
        {
            try
            {
                _orderService.UpdateOrderStatus(arg.OrderId, OrderStatus.PendingPayment);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating order status: " + ex.Message);
                var settingOrderToPendingPaymentFailed = new SettingOrderToPendingPaymentFailed
                {
                    OrderId = arg.OrderId,
                    Reason = ex.Message
                };
                await _messageClient.PublishAsync(settingOrderToPendingPaymentFailed);
            }
            finally
            {
                var orderSetToPendingPayment = new OrderSetToPendingPayment
                {
                    OrderId = arg.OrderId
                };
                await _messageClient.PublishAsync(orderSetToPendingPayment);
            }
        }
        else if (arg.IsReleased.HasValue && arg.IsReleased.Value)
        {
            try
            {
                _orderService.UpdateOrderStatus(arg.OrderId, OrderStatus.PendingRefund);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating order status: " + ex.Message);
                var orderCancellationFailed = new OrderCancellationFailed()
                {
                    OrderId = arg.OrderId,
                    Reason = ex.Message
                };
                await _messageClient.PublishAsync(orderCancellationFailed);
            }
            finally
            {
                var orderCancelled = new OrderCancelled
                {
                    OrderId = arg.OrderId
                };
                await _messageClient
                    .PublishAsync(
                        orderCancelled); //TODO vi skal sørge for at Saga ved den skal refunderer når den får dette event
            }
        }
    }

    #endregion

    #region PaymentSucceeded

    private async Task PaymentSucceeded(PaymentSucceeded arg)
    {
        try
        {
            _orderService.UpdateOrderStatus(arg.OrderId, OrderStatus.Confirmed);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error updating order status: " + ex.Message);
            var orderConfirmFailed = new OrderConfirmFailed
            {
                OrderId = arg.OrderId,
                Reason = ex.Message
            };
            await _messageClient.PublishAsync(orderConfirmFailed);
        }
        finally
        {
            var orderSetToCompleted = new OrderConfirmed()
            {
                OrderId = arg.OrderId
            };
            await _messageClient.PublishAsync(orderSetToCompleted);
        }
    }

    #endregion

    #region PaymentRefunded

    private async Task PaymentRefunded(PaymentRefunded arg)
    {
        try
        {
            _orderService.UpdateOrderStatus(arg.OrderId, OrderStatus.CancelledConfirm);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error updating order status: " + ex.Message);
            var orderCancellationFailed = new OrderCancellationFailed()
            {
                OrderId = arg.OrderId,
                Reason = ex.Message
            };
            await _messageClient.PublishAsync(orderCancellationFailed);
        }
        finally
        {
            var orderCancelled = new OrderCancelled
            {
                OrderId = arg.OrderId
            };
            await _messageClient.PublishAsync(orderCancelled);
        }
    }

    #endregion
    
    
}