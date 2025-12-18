using Microsoft.EntityFrameworkCore;
using StockService.Core.Entities;
using StockService.Core.Interfaces;

namespace StockService.Infrastructure.Repositories;

public class StockRepository : IStockRepository
{
    private readonly AppDbContext _context;

    public StockRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Stock>> GetAllAsync()
    {
        return await _context.Stocks
            .Include(s => s.Product)
            .ToListAsync();
    }

    public async Task<Stock> GetByIdAsync(int id)
    {
        var stock = await _context.Stocks
            .Include(s => s.Product)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (stock == null)
        {
            Console.WriteLine("Stock does not exist");
            throw new KeyNotFoundException("Stock not found for the given StockId");
        }

        return stock;
    }

    public async Task<Stock> CreateAsync(Stock stock)
    {
        var createdStock = await _context.Stocks.AddAsync(stock);
        await _context.SaveChangesAsync();
        return createdStock.Entity;
    }

    public async Task<Stock> UpdateAsync(Stock stock)
    {
        var updatedStock = await _context.Stocks.FindAsync(stock.Id);

        updatedStock.Quantity = stock.Quantity;
        updatedStock.ReservedQuantity = stock.ReservedQuantity;

        await _context.SaveChangesAsync();

        return updatedStock;
    }

    public async Task<Stock> DeleteAsync(int id)
    {
        var deletedStock = await _context.Stocks.FindAsync(id);
        _context.Stocks.Remove(deletedStock);
        await _context.SaveChangesAsync();
        return deletedStock;
    }

    public async Task FreeProductReservation(int argProductId, int argQuantity)
    {
        var stock = await _context.Stocks
            .FirstOrDefaultAsync(s => s.ProductId == argProductId);

        if (stock == null)
        {
            Console.WriteLine("Stock does not exist");
            throw new KeyNotFoundException("Stock not found for the given ProductId");
        }

        stock.ReservedQuantity -= argQuantity;
        stock.Quantity += argQuantity;

        await _context.SaveChangesAsync();
    }

    public async Task ReturnStock(Dictionary<int, int> productsAndQuantities)
    {
        foreach (var pq in productsAndQuantities)
        {
            var stock = await _context.Stocks
                .FirstOrDefaultAsync(s => s.ProductId == pq.Key);

            if (stock == null)
            {
                Console.WriteLine("Stock does not exist");
                throw new KeyNotFoundException("Stock not found for the given ProductId");
            }

            stock.SoldQuantity -= pq.Value;
            stock.Quantity += pq.Value;
        }

        await _context.SaveChangesAsync();
    }

    public async Task SellStock(Dictionary<int, int> productsAndQuantities)
    {
        foreach (var pq in productsAndQuantities)
        {
            var stock = await _context.Stocks
                .FirstOrDefaultAsync(s => s.ProductId == pq.Key);

            if (stock == null)
            {
                Console.WriteLine("Stock does not exist");
                throw new KeyNotFoundException("Stock not found for the given ProductId");
            }

            stock.ReservedQuantity -= pq.Value;
            stock.SoldQuantity += pq.Value;
        }

        await _context.SaveChangesAsync();
    }

    public async Task CancelStock(Dictionary<int, int> productsAndQuantities)
    {
        foreach (var pq in productsAndQuantities)
        {
            var stock = await _context.Stocks
                .FirstOrDefaultAsync(s => s.ProductId == pq.Key);

            if (stock == null)
            {
                Console.WriteLine("Stock does not exist");
                throw new KeyNotFoundException("Stock not found for the given ProductId");
            }

            stock.ReservedQuantity += pq.Value;
            stock.SoldQuantity -= pq.Value;
        }

        await _context.SaveChangesAsync();
    }

    public async Task ReserveStockForProduct(int productId, int quantity)
    {
        var stock = await _context.Stocks
            .FirstOrDefaultAsync(s => s.ProductId == productId);

        if (stock == null)
        {
            Console.WriteLine("Stock does not exist");
            throw new KeyNotFoundException("Stock not found for the given ProductId");
        }

        if (stock.Quantity < quantity)
        {
            Console.WriteLine("Insufficient stock quantity to reserve the requested amount");
            throw new InvalidOperationException("Insufficient stock quantity to reserve the requested amount");
        }

        stock.Quantity -= quantity;
        stock.ReservedQuantity += quantity;

        await _context.SaveChangesAsync();
    }
}