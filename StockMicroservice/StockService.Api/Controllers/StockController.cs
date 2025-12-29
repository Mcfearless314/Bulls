using Microsoft.AspNetCore.Mvc;
using StockService.Core.Entities;
using StockService.DTOs;

namespace StockService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockController : ControllerBase
{
    private readonly Application.Services.StockService _stockService;

    public StockController(Application.Services.StockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var stocks = await _stockService.GetAllAsync();
        return Ok(stocks);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var stock = await _stockService.GetByIdAsync(id);
        return Ok(stock);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateStock([FromBody] StockDto dto)
    {
        var stock = new Stock
        {
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
        };
        var createdStock = await _stockService.CreateAsync(stock);
        return Ok(createdStock);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateStock([FromBody] StockDto dto)
    {
        var stock = new Stock
        {
            Id = dto.Id,
            Quantity = dto.Quantity,
        };
        var updatedStock = await _stockService.UpdateAsync(stock);
        return Ok(updatedStock);
    }
}