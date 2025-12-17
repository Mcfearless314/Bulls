using System.Text;
using EasyNetQ;
using EasyNetQ.Topology;
using System.Text.Json;
using OrderSagaService.Interfaces;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace OrderSaga.Messaging;

public class EasyNetQMessageClient : IMessageClient
{
    private readonly IBus _bus;

    public EasyNetQMessageClient(IBus bus)
    {
        _bus = bus;
    }

    public async Task PublishAsync<T>(T @event) where T : class
    {
        Console.WriteLine($"Publishing event: {typeof(T).Name} - {JsonSerializer.Serialize(@event)}");
        await _bus.PubSub.PublishAsync(@event);
    }
    
    public Task SubscribeTestAsync<T>(
        string subscriptionId,
        Func<T, Task> handler,
        CancellationToken cancellationToken,
        string exchangeName
    ) where T : class
    {
        var exchange = _bus.Advanced.ExchangeDeclare(
            exchangeName,
            ExchangeType.Fanout
        );

        var queue = _bus.Advanced.QueueDeclare(subscriptionId);

        _bus.Advanced.Bind(exchange, queue, "");

        _bus.Advanced.Consume<byte[]>(queue, async (msg, _) =>
        {
            var json = Encoding.UTF8.GetString(msg.Body);
            var data = JsonSerializer.Deserialize<T>(json)!;
            await handler(data);
        });

        return Task.CompletedTask;
    }



    public async Task SubscribeAsync<T>(string subscriptionId, Func<T, Task> onMessage, CancellationToken cancellationToken) where T : class
    {
        Console.WriteLine($"Subscribing to event: {typeof(T).Name} with subscription ID: {subscriptionId}");
        await _bus.PubSub.SubscribeAsync(subscriptionId, onMessage, cancellationToken);
    }
}