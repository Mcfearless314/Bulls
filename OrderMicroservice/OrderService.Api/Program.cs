using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using OrderService.Core.Interfaces;
using OrderService.Infrastructure;
using OrderService.Infrastructure.Repositories;
using OrderService.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IMessageClient, EasyNetQMessageClient>();
builder.Services.AddScoped<OrderService.Application.Services.OrderService>();

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
