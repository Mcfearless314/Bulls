using PaymentService.Core.Contracts;
using PaymentService.Core.DomainEvents;
using PaymentService.Core.Exchanges;
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
            cancellationToken, PaymentEvent.PaymentRequestEvent);

        await _messageClient.SubscribeAsync<PaymentRefundEvent>(
            "payment-refund-subscription",
            HandlePaymentRefund,
            cancellationToken, PaymentEvent.PaymentRefundEvent);
    }

    private async Task HandlePayment(PaymentRequestEvent arg)
    {
        Console.WriteLine("Processing payment request");
        try
        {
            var isPaid = _paymentService.CreatePayment(arg.OrderId, arg.UserId, arg.Amount);

            if (isPaid)
            {
                var paymentSucceeded = new PaymentSucceeded
                {
                    OrderId = arg.OrderId,
                    UserId = arg.UserId,
                    Amount = arg.Amount,
                };
                await _messageClient.PublishAsync(paymentSucceeded, PaymentEvent.PaymentSucceeded);
            }
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
            await _messageClient.PublishAsync(paymentFailed, PaymentEvent.PaymentFailed);
        }
    }

    private async Task HandlePaymentRefund(PaymentRefundEvent arg)
    {
        Console.WriteLine("Processing payment refund request");
        try
        {
            await _paymentService.RefundPayment(arg.OrderId, arg.UserId);
            
            var paymentRefunded = new PaymentRefunded
            {
                OrderId = arg.OrderId,
                UserId = arg.UserId,
                Amount = arg.Amount
            };
            await _messageClient.PublishAsync(paymentRefunded, PaymentEvent.PaymentRefunded);
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
            await _messageClient.PublishAsync(paymentRefundFailed, PaymentEvent.PaymentRefundFailed);
        }
    }
}