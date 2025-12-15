using StockService.Core.Entities;

namespace StockService.Core.Interfaces;

public interface IStockRepository
{
    public Task<IEnumerable<Stock>> GetAllAsync();
    public Task<Stock> GetByIdAsync(int id);
    public Task<Stock> CreateAsync(Stock stock);
    public Task<Stock> UpdateAsync(Stock stock);
    public Task<Stock> DeleteAsync(int id);
}