using System.Text;
using EasyNetQ;
using EasyNetQ.Topology;
using StockService.Core.Interfaces;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace StockService.Messaging;

public class EasyNetQMessageClient : IMessageClient
{
    private readonly IBus _bus;

    public EasyNetQMessageClient(IBus bus)
    {
        _bus = bus;
    }
    
    public async Task PublishAsync<T>(T @event, string exchangeName) where T : class
    {
        Console.WriteLine($"Publishing event: {typeof(T).Name} - {JsonSerializer.Serialize(@event)}");

        var exchange = await _bus.Advanced.ExchangeDeclareAsync(
            exchangeName,
            ExchangeType.Fanout,
            durable: true
        );

        var json = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(json);
        var msg = new Message<byte[]>(body);

        await _bus.Advanced.PublishAsync(
            exchange,
            routingKey: "",
            mandatory: false,
            message: msg
        );
    }
    

    public async Task SubscribeAsync<T>( string subscriptionId, Func<T, Task> handler, CancellationToken cancellationToken, string exchangeName) where T : class
    {
        var exchange = await _bus.Advanced.ExchangeDeclareAsync(
            exchangeName,
            ExchangeType.Fanout, cancellationToken: cancellationToken);

        var queue = await _bus.Advanced.QueueDeclareAsync(subscriptionId, cancellationToken: cancellationToken);

        await _bus.Advanced.BindAsync(exchange, queue, "", cancellationToken: cancellationToken);

        _bus.Advanced.Consume<byte[]>(queue, async (msg, info) =>
        {
            var json = Encoding.UTF8.GetString(msg.Body);
            var data = JsonSerializer.Deserialize<T>(json)!;
            await handler(data);
        });
    }
}