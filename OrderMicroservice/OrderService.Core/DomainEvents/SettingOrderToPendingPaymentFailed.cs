namespace OrderService.Core.DomainEvents;

public class SettingOrderToPendingPaymentFailed
{
    public Guid OrderId { get; set; }
    public required string Reason { get; set; }
}