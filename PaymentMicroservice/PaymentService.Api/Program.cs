using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using PaymentService.Core.Interfaces;
using PaymentService.Infrastructure;
using PaymentService.Messaging;
using PaymentService.Messaging.MessageBackgroundService;
using PaymentService.Messaging.MessageHandler;

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

builder.Services.AddScoped<IPaymentRepository, IPaymentRepository>();
builder.Services.AddScoped<PaymentService.Application.Services.PaymentService>();
builder.Services.AddScoped<IMessageClient, EasyNetQMessageClient>();
builder.Services.AddScoped<IMessageHandler, MessageHandler>();
builder.Services.AddHostedService<MessageBackgroundService>();

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
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.Run();

