using StockService.Core.Contracts;

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
            new Product {Name = "Laptop", Description = "This is a laptop", Price = 5000},
            new Product {Name = "Graphics Card", Description = "This is a graphics card", Price = 500},
        };
        
        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
        
        var Stock = new List<Stock>
        {
            new Stock {ProductId = 1, Quantity = 10},
            new Stock {ProductId = 2, Quantity = 10},
            new Stock {ProductId = 3, Quantity= 50},
        };
        
        await context.Stocks.AddRangeAsync(Stock);
        await context.SaveChangesAsync();
    }
}