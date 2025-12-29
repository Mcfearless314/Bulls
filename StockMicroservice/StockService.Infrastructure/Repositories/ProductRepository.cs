using Microsoft.EntityFrameworkCore;
using StockService.Core.Entities;
using StockService.Core.Interfaces;

namespace StockService.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly StockDbContext _context;

    public ProductRepository(StockDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<Product> GetByIdAsync(int id)
    {
        var result = await _context.Products
            .FromSqlInterpolated($"EXEC dbo.GetProductById @Id = {id}")
            .ToListAsync(); 

        var product = result.FirstOrDefault();
        if (product == null)
            throw new KeyNotFoundException("Product not found");

        return product;
    }


    public async Task<Product> CreateAsync(Product product)
    {
        var cratedProduct = await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return cratedProduct.Entity;
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        var updatedProduct = await _context.Products.FindAsync(product.Id);

        updatedProduct.Name = product.Name;
        updatedProduct.Description = product.Description;
        updatedProduct.Price = product.Price;

        await _context.SaveChangesAsync();

        return updatedProduct;
    }

    public async Task<Product> DeleteAsync(int id)
    {
        var deletedProduct = await _context.Products.FindAsync(id);
        _context.Products.Remove(deletedProduct);
        await _context.SaveChangesAsync();
        return deletedProduct;
    }
}