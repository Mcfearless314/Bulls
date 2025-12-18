namespace OrderSagaService.State;

public class RemovalOfOrderItemFromOrderSagaState
{
    public Guid SagaId { get; set; }
    public Guid OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public int StockRetryCount { get; set; }
}