using PaymentService.Core.Contracts;

namespace PaymentService.Core.Interfaces;

public interface IMessageHandler
{
    public Task Subscribe(CancellationToken cancellationToken);
    public Task Handle(PaymentRequestEvent arg);
}