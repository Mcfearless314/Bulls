namespace StockService.Infrastructure.DTOs;

public class StockWithProductDto
{
    // Stock fields
    public int Id { get; set; }    
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public int ReservedQuantity { get; set; }
    public int SoldQuantity { get; set; }

    // Product fields
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
}
