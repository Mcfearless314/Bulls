using StockService.Core.Contracts;
using StockService.Core.Entities;
using StockService.Core.Interfaces;

namespace StockService.Application.Services;

public class StockService
{
    private readonly IStockRepository _stockRepository;
    
    public StockService(IStockRepository stockRepository)
    {
        _stockRepository = stockRepository;
    }

    public async Task<IEnumerable<Stock>> GetAllAsync()
    {
        return await _stockRepository.GetAllAsync();
    }

    public async Task<Stock> GetByIdAsync(int id)
    {
        return await _stockRepository.GetByIdAsync(id);
    }

    public async Task<Stock> CreateAsync(Stock stock)
    {
        return await _stockRepository.CreateAsync(stock);
    }

    public async Task<Stock> UpdateAsync(Stock stock)
    {
        return await _stockRepository.UpdateAsync(stock);
    }

    public async Task<Stock> DeleteAsync(int id)
    {
        return await _stockRepository.DeleteAsync(id);
    }

    public async Task FreeProductReservation(FreeProductReservationEvent arg)
    {
        await _stockRepository.FreeProductReservation(arg.ProductId, arg.Quantity);

    }

    public async Task ReturnStock(CancelStockEvent arg)
    {
        await _stockRepository.ReturnStock(arg.ProductsAndQuantities);
    }

    public async Task SellStock(SellStockEvent arg)
    {
        await _stockRepository.SellStock(arg.ProductsAndQuantities);
    }

    public async Task ReserveStockForProduct(ReserveProductEvent reserveProductEvent)
    {
        await _stockRepository.ReserveStockForProduct(reserveProductEvent.ProductId, reserveProductEvent.Quantity);
    }
}