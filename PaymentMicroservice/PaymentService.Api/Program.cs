using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PaymentService.Core.Interfaces;
using PaymentService.Infrastructure;
using PaymentService.Infrastructure.Repositories;
using PaymentService.Messaging;
using PaymentService.Messaging.MessageBackgroundService;
using PaymentService.Messaging.MessageHandler;
using PaymentService.Vault;
using Vault;
using Vault.Client;
using Vault.Model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var vaultHostName = builder.Configuration.GetValue<string>("Vault:VaultHostName");
var vaultPath = builder.Configuration.GetValue<string>("Vault:VaultPath");
var vaultKvV2MountPath = builder.Configuration.GetValue<string>("Vault:VaultKvV2MountPath");

if (string.IsNullOrEmpty(vaultHostName) || string.IsNullOrEmpty(vaultPath) || string.IsNullOrEmpty(vaultKvV2MountPath))
{
    throw new InvalidOperationException("Missing required Vault configuration values. Please ensure 'Vault:VaultHostName', 'Vault:VaultPath', and 'Vault:VaultKvV2MountPath' are set in configuration.");
}

builder.Configuration.AddEnvironmentVariables();
var vaultUsername = builder.Configuration["Username"];
var vaultPassword = builder.Configuration["Password"];

if (string.IsNullOrWhiteSpace(vaultUsername) || string.IsNullOrWhiteSpace(vaultPassword))
{
    Console.Error.WriteLine("Vault credentials are missing. Ensure environment variables 'Username' and 'Password' are set.");
    throw new InvalidOperationException("Vault credentials are required to start the API Gateway.");
}

var secretSettings = new SecretSettings();
builder.Services.AddSingleton(secretSettings);

while (string.IsNullOrEmpty(secretSettings.PaymentDb))
{
    try
    {
        secretSettings = VaultHelper.FetchSecretsFromVault(vaultHostName, vaultPath, vaultKvV2MountPath, vaultUsername, vaultPassword);
        Console.WriteLine("Successfully fetched secrets from Vault.");
    }
    catch (Exception exception)
    {
        Console.WriteLine($"Error fetching secrets from Vault: {exception.Message}");
        Thread.Sleep(5000);
    }
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(secretSettings.PaymentDb, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);
    }));

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<PaymentService.Application.Services.PaymentService>();
builder.Services.AddScoped<IMessageClient, EasyNetQMessageClient>();
builder.Services.AddScoped<IMessageHandler, MessageHandler>();
builder.Services.AddHostedService<MessageBackgroundService>();

builder.Services.AddSingleton<IBus>(sp =>
{

    for (int i = 0; i < 10; i++)
    {
        try
        {
            var bus = RabbitHutch.CreateBus($"host=rmq;virtualHost=/;username={secretSettings.RmqUser};password={secretSettings.RmqPassword}");
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

