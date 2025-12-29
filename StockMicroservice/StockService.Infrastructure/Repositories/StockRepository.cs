using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StockService.Core.Entities;
using StockService.Core.Interfaces;
using StockService.Infrastructure.DTOs;

namespace StockService.Infrastructure.Repositories;

public class StockRepository : IStockRepository
{
    private readonly StockDbContext _context;

    public StockRepository(StockDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Stock>> GetAllAsync()
    {
        var result = await _context.Database.SqlQuery<StockWithProductDto>($"EXEC dbo.GetAllStocks")
            .ToListAsync();

        var stocks = result.Select(d => new Stock
        {
            Id = d.Id,
            ProductId = d.ProductId,
            Quantity = d.Quantity,
            ReservedQuantity = d.ReservedQuantity,
            SoldQuantity = d.SoldQuantity,
            Product = new Product
            {
                Id = d.ProductId,
                Name = d.Name,
                Description = d.Description,
                Price = d.Price
            }
        }).ToList();

        return stocks;
    }


    public async Task<Stock> GetByIdAsync(int id)
    {
        var result = await _context.Database
            .SqlQuery<StockWithProductDto>($"EXEC dbo.GetStockById @Id = {id}")
            .ToListAsync();   

        var dto = result.FirstOrDefault();  

        if (dto == null)
        {
            Console.WriteLine("Stock does not exist");
            throw new KeyNotFoundException("Stock not found for the given StockId");
        }

        var stock = new Stock
        {
            Id = dto.Id,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            ReservedQuantity = dto.ReservedQuantity,
            SoldQuantity = dto.SoldQuantity,
            Product = new Product
            {
                Id = dto.ProductId,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price
            }
        };

        return stock;
    }

    public async Task<Stock> CreateAsync(Stock stock)
    {
        var result = await _context.Database
            .SqlQuery<StockWithProductDto>(
                $"EXEC dbo.CreateStock @ProductId = {stock.ProductId}, @Quantity = {stock.Quantity}")
            .ToListAsync();

        var dto = result.FirstOrDefault();

        if (dto == null)
            throw new KeyNotFoundException("CreateStock did not return data");

        return new Stock
        {
            Id = dto.Id,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            ReservedQuantity = dto.ReservedQuantity,
            SoldQuantity = dto.SoldQuantity,
            Product = new Product
            {
                Id = dto.ProductId,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price
            }
        };
    }

    public async Task<Stock> UpdateAsync(Stock stock)
    {
        var result = await _context.Database
            .SqlQuery<StockWithProductDto>($"EXEC dbo.UpdateStockQuantity @Id = {stock.Id}, @Quantity = {stock.Quantity}")
            .ToListAsync();
        
        var dto = result.FirstOrDefault();

        if (dto == null)
            throw new KeyNotFoundException("UpdateStockQuantity did not return data");

        return new Stock
        {
            Id = dto.Id,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            ReservedQuantity = dto.ReservedQuantity,
            SoldQuantity = dto.SoldQuantity,
            Product = new Product
            {
                Id = dto.ProductId,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price
            }
        };
    }
    
    public async Task FreeProductReservation(int productId, int quantity)
    {
        try
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"EXEC dbo.FreeProductReservation @ProductId = {productId}, @Quantity = {quantity}");
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"SQL Error: {ex.Message}");
            throw new InvalidOperationException("Something went wrong.");
        }
    }

    public async Task ReturnStock(Dictionary<int, int> productsAndQuantities)
    {
        try
        {
            foreach (var pq in productsAndQuantities)
            {
                await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"EXEC dbo.ReturnStock @ProductId = {pq.Key}, @Quantity = {pq.Value}");
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"SQL Error: {ex.Message}");
            throw new InvalidOperationException("Something went wrong.");
        }
    }


    public async Task SellStock(Dictionary<int, int> productsAndQuantities)
    {
        try
        {
            foreach (var pq in productsAndQuantities)
            {
                await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"EXEC dbo.SellStock @ProductId = {pq.Key}, @Quantity = {pq.Value}");
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"SQL Error: {ex.Message}");
            throw new InvalidOperationException("Something went wrong.");
        }
    }

    public async Task CancelStock(Dictionary<int, int> productsAndQuantities)
    {
        try
        {
            foreach (var pq in productsAndQuantities)
            {
                await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"EXEC dbo.CancelStock @ProductId = {pq.Key}, @Quantity = {pq.Value}");
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"SQL Error: {ex.Message}");
            throw new InvalidOperationException("Something went wrong.");
        }
    }


    public async Task ReserveStockForProduct(int productId, int quantity)
    {
        try
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"EXEC dbo.ReserveStock @ProductId = {productId}, @Quantity = {quantity}");
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"SQL Error: {ex.Message}");
            throw new InvalidOperationException("Something went wrong.");
        }
    }
}