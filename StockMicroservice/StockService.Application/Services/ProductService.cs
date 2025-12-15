using StockService.Core.Entities;
using StockService.Core.Interfaces;

namespace StockService.Application.Services;

public class ProductService
{
    private readonly IProductRepository _productRepository;
    
    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _productRepository.GetAllAsync();
    }

    public async Task<Product> CreateAsync(Product product)
    {
        return await _productRepository.CreateAsync(product);
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        return await _productRepository.UpdateAsync(product);
    }

    public async Task<Product> DeleteAsync(int id)
    {
        return await _productRepository.DeleteAsync(id);
    }
}