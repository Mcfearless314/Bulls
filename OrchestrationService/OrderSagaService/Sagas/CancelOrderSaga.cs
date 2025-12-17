using OrderSagaService.Contracts.Commands.Stock;
using OrderSagaService.Contracts.Responses.Order;
using OrderSagaService.Interfaces;
using OrderSagaService.State;

namespace OrderSagaService.Sagas;

public class CancelOrderSaga
{
    private readonly IMessageClient _messageClient;

    public CancelOrderSaga(IMessageClient messageClient)
    {
        _messageClient = messageClient;
    }

    public async Task Handle(object message)
    {
        switch (message)
        {
            case OrderCancelled e:
                await Handle(e);
                break;
        }
    }
    
    private async Task Handle(OrderCancelled evt)
    {
        var saga = new OrderSagaState
        {
            SagaId = Guid.NewGuid(),
            OrderId = evt.OrderId, 
            Action = ActionType.OrderCancelled
        };
    }
}