using PaymentService.Core.Contracts;
using PaymentService.Core.DomainEvents;
using PaymentService.Core.Interfaces;

namespace PaymentService.Messaging.MessageHandler;

public class MessageHandler : IMessageHandler
{
    private readonly IMessageClient _messageClient;
    private readonly Application.Services.PaymentService _paymentService;

    public MessageHandler(IMessageClient messageClient, Application.Services.PaymentService paymentService)
    {
        _messageClient = messageClient;
        _paymentService = paymentService;
    }

    public async Task Subscribe(CancellationToken cancellationToken)
    {
        await _messageClient.SubscribeAsync<PaymentRequestEvent>(
            "payment-service-subscription",
            HandlePayment,
            cancellationToken);

        await _messageClient.SubscribeAsync<PaymentRefundEvent>(
            "payment-refund-subscription",
            HandlePaymentRefund,
            cancellationToken);
    }

    private async Task HandlePayment(PaymentRequestEvent arg)
    {
        Console.WriteLine("Processing payment request");
        var isPaid = false;
        try
        {
            isPaid = _paymentService.CreatePayment(arg.OrderId, arg.UserId, arg.Amount);
        }
        catch (Exception ex)
        {
            var paymentFailed = new PaymentFailed
            {
                OrderId = arg.OrderId,
                UserId = arg.UserId,
                Amount = arg.Amount,
                Reason = ex.Message
            };
            await _messageClient.PublishAsync(paymentFailed);
        }
        finally
        {
            if (isPaid)
            {
                var paymentSucceeded = new PaymentSucceeded
                {
                    OrderId = arg.OrderId,
                    UserId = arg.UserId,
                    Amount = arg.Amount,
                };
                await _messageClient.PublishAsync(paymentSucceeded);
            }
        }
    }

    private async Task HandlePaymentRefund(PaymentRefundEvent arg)
    {
        Console.WriteLine("Processing payment refund request");
        try
        {
            await _paymentService.RefundPayment(arg.OrderId, arg.UserId);
        }
        catch (Exception ex)
        {
            var paymentRefundFailed = new PaymentRefundFailed
            {
                OrderId = arg.OrderId,
                UserId = arg.UserId,
                Amount = arg.Amount,
                Reason = ex.Message
            };
            await _messageClient.PublishAsync(paymentRefundFailed);
        }
        finally
        {
            var paymentRefunded = new PaymentRefunded
            {
                OrderId = arg.OrderId,
                UserId = arg.UserId,
                Amount = arg.Amount
            };
            await _messageClient.PublishAsync(paymentRefunded);
        }
    }
}