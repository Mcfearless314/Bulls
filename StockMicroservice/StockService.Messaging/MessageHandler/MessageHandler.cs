using StockService.Core.Interfaces;
using StockService.Application.Services;

namespace StockService.Messaging.MessageHandler;

public class MessageHandler
{
    private readonly IMessageClient _messagingClient;
    private readonly Application.Services.StockService _stockService;
    private readonly ProductService _productService;
    
    public MessageHandler(IMessageClient messagingClient, Application.Services.StockService stockService, ProductService productService)
    {
        _messagingClient = messagingClient;
        _stockService = stockService;
        _productService = productService;
    }
    
    
}