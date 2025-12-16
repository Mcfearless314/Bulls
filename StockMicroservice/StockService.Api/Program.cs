using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using StockService.Application.Services;
using StockService.Core.Interfaces;
using StockService.Infrastructure;
using StockService.Infrastructure.Repositories;
using StockService.Messaging;
using StockService.Messaging.MessageBackgroundService;
using StockService.Messaging.MessageHandler;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);
    }));

builder.Services.AddScoped<DbInitializer>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<StockService.Application.Services.StockService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<IMessageClient, EasyNetQMessageClient>();
builder.Services.AddScoped<IMessageHandler, MessageHandler>();
builder.Services.AddHostedService<MessageBackgroundService>();

builder.Services.AddSingleton<IBus>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var rmqPassword = configuration.GetSection("rmq")["password"]; // TODO: Figure out why with the secrets.json file
    var rmqUsername = configuration.GetSection("rmq")["username"]; // TODO: Figure out why with the secrets.json file
    
    for (int i = 0; i < 10; i++)
    {
        try
        {
            var bus = RabbitHutch.CreateBus($"host=rmq;virtualHost=/;username=guest;password=guest"); // TODO: Figure out why with the secrets.json file
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

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetService<AppDbContext>();
    var dbInitializer = services.GetRequiredService<DbInitializer>();
    dbInitializer.InitializeAsync(dbContext).GetAwaiter().GetResult();
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.UseHttpsRedirection();

app.Run();
