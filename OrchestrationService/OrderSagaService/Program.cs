using EasyNetQ;
using OrderSagaService.Interfaces;
using OrderSagaService.Messaging;
using OrderSagaService.Sagas;
using OrderSagaService.Vault;
using OrderSagaService.Workers;

var builder = Host.CreateApplicationBuilder(args);

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

while (string.IsNullOrEmpty(secretSettings.RmqUser))
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
builder.Services.AddScoped<IMessageClient, EasyNetQMessageClient>();

builder.Services.AddScoped<PlaceOrderSaga>();
builder.Services.AddScoped<AddOrderItemToOrderSaga>();
builder.Services.AddScoped<RemovalOfOrderItemFromOrderSaga>();

builder.Services.AddHostedService<PlaceOrderSagaBackgroundService>();
builder.Services.AddHostedService<AddOrderItemToOrderSagaBackgroundService>();
builder.Services.AddHostedService<RemovalOfOrderItemFromOrderSagaBackgroundService>();

var host = builder.Build();
host.Run();