namespace OrderSagaService.Contracts.Commands.Order;

public class ConfirmOrderEvent
{
    public Guid OrderId { get; set; }
}