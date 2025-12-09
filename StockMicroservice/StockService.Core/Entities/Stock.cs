namespace StockService.Core.Contracts;

public class Stock
{
    public int Id { get; set; }    
    public Product Product { get; set; }
    public int Quantity { get; set; }
    public int ReservedQuantity { get; set; }
}