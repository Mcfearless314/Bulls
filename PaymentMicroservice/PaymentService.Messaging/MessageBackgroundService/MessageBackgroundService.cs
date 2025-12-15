using Microsoft.Extensions.Hosting;
using PaymentService.Core.Interfaces;

namespace PaymentService.Messaging.MessageBackgroundService;

public class MessageBackgroundService : BackgroundService
{
    private readonly IMessageHandler _messageHandler;

    public MessageBackgroundService(IMessageHandler messageHandler)
    {
        _messageHandler = messageHandler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var retries = 0;
        const int maxRetries = 10;
        const int delayMs = 5000;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Console.WriteLine("Setting up message handler");
                await _messageHandler.Subscribe(stoppingToken);
                Console.WriteLine("Subscription to channel-event succeeded.");
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Subscription failed: {ex.Message}");
                retries++;

                if (retries >= maxRetries)
                {
                    throw;
                }

                await Task.Delay(delayMs, stoppingToken);
            }

            Console.WriteLine("Message handler is set up");
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }

        Console.WriteLine("MessageBackgroundService is stopping");
    }
}