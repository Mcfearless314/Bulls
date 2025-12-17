namespace OrderSagaService.Interfaces;

public interface IMessageClient
{
    public Task PublishAsync<T>(T @event) where T : class;
    public Task SubscribeTestAsync<T>(string subscriptionId, Func<T, Task> onMessage, CancellationToken cancellationToken, string exchangeName) where T : class;
    public Task SubscribeAsync<T>(string subscriptionId, Func<T, Task> onMessage, CancellationToken cancellationToken) where T : class;
}