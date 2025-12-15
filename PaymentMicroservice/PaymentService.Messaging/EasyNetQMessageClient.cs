using EasyNetQ;
using PaymentService.Core.Interfaces;

namespace PaymentService.Messaging;

public class EasyNetQMessageClient : IMessageClient
{
    private readonly IBus _bus;

    public EasyNetQMessageClient(IBus bus)
    {
        _bus = bus;
    }

    public async Task PublishAsync<T>(T @event) where T : class
    {
        Console.WriteLine($"Publishing event: {typeof(T).Name} - {System.Text.Json.JsonSerializer.Serialize(@event)}");
        await _bus.PubSub.PublishAsync(@event);
    }

    public async Task SubscribeAsync<T>(string subscriptionId, Func<T, Task> onMessage, CancellationToken cancellationToken) where T : class
    {
        Console.WriteLine($"Subscribing to event: {typeof(T).Name} with subscription ID: {subscriptionId}");
        await _bus.PubSub.SubscribeAsync(subscriptionId, onMessage, cancellationToken);
    }
}