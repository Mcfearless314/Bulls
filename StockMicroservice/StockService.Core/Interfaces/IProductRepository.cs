using StockService.Core.Contracts;

namespace StockService.Core.Interfaces;

public interface IProductRepository
{
    public Task<IEnumerable<Product>> GetAllAsync();
    public Task<Product> CreateAsync(Product product);
    public Task<Product> UpdateAsync(Product product);
    public Task<Product> DeleteAsync(int id);
}