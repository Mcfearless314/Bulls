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
        return await _context.Stocks
            .Include(s => s.Product)
            .FirstOrDefaultAsync(s => s.Id == id);
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
}