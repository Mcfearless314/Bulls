using StockService.Core.Entities;

namespace StockService.Infrastructure;

public class DbInitializer
{
    public async Task InitializeAsync(AppDbContext context)
    {
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        var products = new List<Product>
        {
            new Product {Name = "Screen", Description = "This is a screen", Price = 1000},
            new Product {Name = "Laptop", Description = "This is a laptop", Price = 8000},
            new Product {Name = "Graphics Card", Description = "This is a graphics card", Price = 5000},
        };
        
        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
        
        var stock = new List<Stock>
        {
            new Stock {ProductId = 1, Quantity = 10, ReservedQuantity = 2},
            new Stock {ProductId = 2, Quantity = 10, ReservedQuantity = 2},
            new Stock {ProductId = 3, Quantity= 50, ReservedQuantity = 1},
        };
        
        await context.Stocks.AddRangeAsync(stock);
        await context.SaveChangesAsync();
    }
}