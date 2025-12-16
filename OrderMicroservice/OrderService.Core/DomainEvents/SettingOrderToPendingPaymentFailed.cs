namespace OrderService.Core.DomainEvents;

public class SettingOrderToPendingPaymentFailed
{
    public int OrderId { get; set; }
    public required string Reason { get; set; }
}