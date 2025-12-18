namespace OrderSagaService.State;

public class AddOrderItemToOrderSagaState
{
    public Guid SagaId { get; set; }
    public Guid OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public int StockRetryCount { get; set; }
}