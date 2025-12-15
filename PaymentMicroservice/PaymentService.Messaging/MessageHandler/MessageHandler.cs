using PaymentService.Core.Contracts;
using PaymentService.Core.Interfaces;

namespace PaymentService.Messaging.MessageHandler;

public class MessageHandler : IMessageHandler
{

    private readonly IMessageClient _messageClient;
    public MessageHandler(IMessageClient messageClient)
    {
        _messageClient = messageClient;
    }

    public async Task Subscribe(CancellationToken cancellationToken)
    {
        await _messageClient.SubscribeAsync<PaymentRequestEvent>(
            "payment-service-subscription",
            Handle,
            cancellationToken);
    }

    public async Task Handle(PaymentRequestEvent arg)
    {
        throw new NotImplementedException();
    }
}