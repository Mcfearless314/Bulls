namespace PaymentService.Core.Interfaces;

public interface IMessageClient
{
    public Task PublishAsync<T>(T @event) where T : class;
    public Task SubscribeAsync<T>(string subscriptionId, Func<T, Task> onMessage, CancellationToken cancellationToken) where T : class;
}