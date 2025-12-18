using StockService.Core.Interfaces;
using StockService.Application.Services;
using StockService.Core.Contracts;
using StockService.Core.DomainEvents;
using StockService.Core.Entities;
using StockService.Core.Exchanges;

namespace StockService.Messaging.MessageHandler;

public class MessageHandler : IMessageHandler
{
    private readonly IMessageClient _messagingClient;
    private readonly Application.Services.StockService _stockService;
    private readonly ProductService _productService;

    public MessageHandler(IMessageClient messagingClient, Application.Services.StockService stockService,
        ProductService productService)
    {
        _messagingClient = messagingClient;
        _stockService = stockService;
        _productService = productService;
    }

    public async Task Subscribe(CancellationToken cancellationToken)
    {
        await _messagingClient.SubscribeAsync<FreeProductReservationEvent>(StockEvent.FreeProductReservationEvent,
            FreeProductReservation, cancellationToken, StockEvent.FreeProductReservationEvent);

        await _messagingClient.SubscribeAsync<CancelStockEvent>(StockEvent.CancelStockEvent,
            ReleaseStock, cancellationToken, StockEvent.CancelStockEvent);

        await _messagingClient.SubscribeAsync<ReserveProductEvent>(StockEvent.ReserveProductEvent, ReserveProduct,
            cancellationToken, StockEvent.ReserveProductEvent);

        await _messagingClient.SubscribeAsync<SellStockEvent>(StockEvent.SellStockEvent,
            SellStock, cancellationToken, StockEvent.SellStockEvent);
    }


    #region FreeProductReservation

    private async Task FreeProductReservation(FreeProductReservationEvent arg)
    {
        try
        {
            await _stockService.FreeProductReservation(arg);
            
            var stockReleased = new StockReleased
            {
                OrderId = arg.OrderId
            };
            await _messagingClient.PublishAsync(stockReleased, StockEvent.StockReleased);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling FreeProductReservationEvent: {ex.Message}");
            var stockReleasedFailed = new StockReleasedFailed
            {
                OrderId = arg.OrderId,
                Reason = ex.Message
            };
            await _messagingClient.PublishAsync(stockReleasedFailed, StockEvent.FreeProductReservationEvent);
        }
    }

    #endregion

    #region ReleaseStock

    private async Task ReleaseStock(CancelStockEvent arg)
    {
        Console.WriteLine("Return stock triggered");
        try
        {
            await _stockService.CancelStock(arg);
            
            var stockCancelled = new StockCancelled
            {
                OrderId = arg.OrderId,
            };
            await _messagingClient.PublishAsync(stockCancelled, StockEvent.StockCancelled);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling ReleaseStockEvent: {ex.Message}");
            var stockCancelledFailed = new StockCancelledFailed
            {
                OrderId = arg.OrderId,
                Reason = ex.Message
            };
            await _messagingClient.PublishAsync(stockCancelledFailed, StockEvent.StockCancelledFailed);
        }
    }

    #endregion

    #region SellStock

    private async Task SellStock(SellStockEvent arg)
    {
        Console.WriteLine("Sell stock triggered");
        try
        {
            await _stockService.SellStock(arg);
            
            var stockSold = new StockSold
            {
                OrderId = arg.OrderId,
            };
            await _messagingClient.PublishAsync(stockSold, StockEvent.StockSold);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling UpdateStockEvent: {ex.Message}");
            var stockSoldFailed = new StockSoldFailed
            {
                OrderId = arg.OrderId,
                Reason = ex.Message
            };
            await _messagingClient.PublishAsync(stockSoldFailed, StockEvent.StockSoldFailed);
        }
    }

    #endregion

    #region ReserveProduct

    private async Task ReserveProduct(ReserveProductEvent arg)
    {
        Product product;
        try
        {
            await _stockService.ReserveStockForProduct(arg);
            product = await _productService.GetByIdAsync(arg.ProductId);
            
            var stockReserved = new StockReserved
            {
                OrderId = arg.OrderId,
                ProductId = product.Id,
                Quantity = arg.Quantity,
                ProductName = product.Name,
                Price = product.Price
            };
            await _messagingClient.PublishAsync(stockReserved, StockEvent.StockReserved);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling ReserveProductEvent: {ex.Message}");
            var stockReserveFailed = new StockReserveFailed
            {
                OrderId = arg.OrderId,
                ProductId = arg.ProductId,
                Quantity = arg.Quantity,
                Reason = ex.Message
            };
            await _messagingClient.PublishAsync(stockReserveFailed, StockEvent.StockReserveFailed);
        }
    }

    #endregion
}