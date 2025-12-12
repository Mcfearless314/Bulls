using Microsoft.AspNetCore.Mvc;
using StockService.Application.Services;
using StockService.Core.Contracts;

namespace StockService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly ProductService _productService;
    
    public  ProductController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(Product product)
    {
        var createdProduct = await _productService.CreateAsync(product);
        return Ok(createdProduct);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProduct(Product product)
    {
        var updatedProduct = await _productService.UpdateAsync(product);
        return Ok(updatedProduct);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var deletedProduct = await _productService.DeleteAsync(id);
        return Ok(deletedProduct);
    }
}