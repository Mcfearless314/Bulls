

namespace StockService.Core.Interfaces;

public interface IMessageHandler
{
    public Task Subscribe(CancellationToken cancellationToken);
    
    public Task HandleMessage();
}