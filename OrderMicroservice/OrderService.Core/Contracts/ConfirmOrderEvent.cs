namespace OrderService.Core.Contracts;

public class ConfirmOrderEvent
{
    public Guid OrderId { get; set; }
}