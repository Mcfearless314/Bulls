using StockService.Core.Interfaces;
using StockService.Application.Services;
using StockService.Core.Contracts;
using StockService.Core.DomainEvents;
using StockService.Core.Entities;

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
        await _messagingClient.SubscribeAsync<FreeProductReservationEvent>("free-product-reservation",
            FreeProductReservation, cancellationToken);

        await _messagingClient.SubscribeAsync<UpdateStockEvent>("update-stock", UpdateStock, cancellationToken);

        await _messagingClient.SubscribeAsync<ReserveProductEvent>("reserve-product", ReserveProduct,
            cancellationToken);
    }


    #region FreeProductReservation

    private async Task FreeProductReservation(FreeProductReservationEvent arg)
    {
        try
        {
            await _stockService.FreeProductReservation(arg);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling FreeProductReservationEvent: {ex.Message}");
            var stockReleasedFailed = new StockReleasedFailed
            {
                OrderId = arg.OrderId,
                Reason = ex.Message
            };
            await _messagingClient.PublishAsync(stockReleasedFailed);
        }
        finally
        {
            var stockReleased = new StockReleased
            {
                OrderId = arg.OrderId
            };
            await _messagingClient.PublishAsync(stockReleased);
        }
    }

    #endregion

    #region UpdateStock

    private async Task UpdateStock(UpdateStockEvent arg)
    {
        if (arg.IsCancelled)
        {
            try
            {
                await _stockService.ReturnStock(arg);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling UpdateStockEvent: {ex.Message}");
                var stockUpdateFailed = new StockUpdateFailed
                {
                    OrderId = arg.OrderId,
                    Reason = ex.Message
                };
                await _messagingClient.PublishAsync(stockUpdateFailed);
            }
            finally
            {
                var stockUpdated = new StockUpdated
                {
                    OrderId = arg.OrderId,
                    IsReleased = true
                };
                await _messagingClient.PublishAsync(stockUpdated);
            }
        }
        else if (arg.IsSold)
        {
            try
            {
                await _stockService.SellStock(arg);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling UpdateStockEvent: {ex.Message}");
                var stockUpdateFailed = new StockUpdateFailed
                {
                    OrderId = arg.OrderId,
                    Reason = ex.Message
                };
                await _messagingClient.PublishAsync(stockUpdateFailed);
            }
            finally
            {
                var stockUpdated = new StockUpdated
                {
                    OrderId = arg.OrderId,
                    IsSold = true
                };
                await _messagingClient.PublishAsync(stockUpdated);
            }
        }
    }

    #endregion

    #region ReserveProduct

    private async Task ReserveProduct(ReserveProductEvent arg)
    {
        var product = new Product();
        try
        {
            await _stockService.ReserveStockForProduct(arg);
            product = await _productService.GetByIdAsync(arg.ProductId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling FreeProductReservationEvent: {ex.Message}");
            var stockReserveFailed = new StockReserveFailed
            {
                OrderId = arg.OrderId,
                ProductId = arg.ProductId,
                Quantity = arg.Quantity,
                Reason = ex.Message
            };
            await _messagingClient.PublishAsync(stockReserveFailed);
        }
        finally
        {
            var stockReserved = new StockReserved
            {
                OrderId = arg.OrderId,
                ProductId = product.Id,
                Quantity = arg.Quantity,
                ProductName = product.Name,
                Price = product.Price
            };
            await _messagingClient.PublishAsync(stockReserved);
        }
    }

    #endregion
}