using System.Runtime.InteropServices;
using StockService.Core.Entities;

namespace StockService.Infrastructure;

public class DbInitializer
{
    public async Task InitializeAsync(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();
        if (!context.Products.Any())
        {
            var products = new List<Product>
            {
                new Product { Name = "Screen", Description = "This is a screen", Price = 1000 },
                new Product { Name = "Laptop", Description = "This is a laptop", Price = 8000 },
                new Product { Name = "Graphics Card", Description = "This is a graphics card", Price = 5000 },
                new Product { Name = "Mouse", Description = "This is a mouse", Price = 200 },
            };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }

        if (!context.Stocks.Any())
        {
            var stock = new List<Stock>
            {
                new Stock { ProductId = 1, Quantity = 20 },
                new Stock { ProductId = 2, Quantity = 30 },
                new Stock { ProductId = 3, Quantity = 50 },
            };

            context.Stocks.AddRange(stock);
            await context.SaveChangesAsync();
        }
    }
}