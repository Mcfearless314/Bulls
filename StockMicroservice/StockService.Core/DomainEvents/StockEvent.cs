using StockService.Core.DomainEvents;

namespace StockService.Core.Interfaces;

public class StockEvent
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public int ReservationId { get; set; }
    public StockActionType Action { get; set; }
}

public enum StockActionType
{
    Reserved = 1,
    RequestStatus = 2,
    Released = 3
}
