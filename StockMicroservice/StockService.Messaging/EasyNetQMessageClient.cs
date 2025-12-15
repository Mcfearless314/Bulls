using EasyNetQ;
using StockService.Core.Interfaces;

namespace StockService.Messaging;

public class EasyNetQMessageClient : IMessageClient
{
    private readonly IBus _bus;

    public EasyNetQMessageClient(IBus bus)
    {
        _bus = bus;
    }
    
    public async Task PublishAsync<T>(T @event) where T : class
    {
    }

    public async Task SubscribeAsync<T>(string subscriptionId, Func<T, Task> onMessage, CancellationToken cancellationToken) where T : class
    {
    }
}