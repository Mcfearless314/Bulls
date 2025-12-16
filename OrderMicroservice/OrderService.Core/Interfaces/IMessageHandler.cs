namespace OrderService.Core.Interfaces;

public interface IMessageHandler
{
    public Task Subscribe(CancellationToken cancellationToken);
    
    
}