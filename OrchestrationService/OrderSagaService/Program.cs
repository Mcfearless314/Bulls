using EasyNetQ;
using OrderSaga.Messaging;
using OrderSagaService.Interfaces;
using OrderSagaService.Sagas;
using OrderSagaService.Workers;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<IBus>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var rmqPassword = configuration.GetSection("rmq")["password"];
    var rmqUsername = configuration.GetSection("rmq")["username"];

    for (int i = 0; i < 10; i++)
    {
        try
        {
            var bus = RabbitHutch.CreateBus($"host=rmq;virtualHost=/;username={rmqUsername};password={rmqPassword}");
            Console.WriteLine($"EasyNetQ connected to rabbitmq on attempt {i+1}");
            return bus;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to connect to RabbitMQ: {ex.Message}");
            Thread.Sleep(5000);
        }
    }

    throw new Exception("Could not connect to RabbitMQ after multiple attempts.");
});
builder.Services.AddScoped<IMessageClient, EasyNetQMessageClient>();
builder.Services.AddScoped<PlaceOrderSaga>();
builder.Services.AddHostedService<PlaceOrderSagaBackgroundService>();
var host = builder.Build();
host.Run();