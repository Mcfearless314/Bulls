using AuthenticationService.Application.Services;
using AuthenticationService.Core.Entities;
using AuthenticationService.Core.Interfaces;
using AuthenticationService.Infrastructure;
using AuthenticationService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

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

while (string.IsNullOrEmpty(secretSettings.AuthDb))
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
    options.UseSqlServer(secretSettings.AuthDb, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);
    }));

builder.Services.AddScoped<DbInitializer>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddControllers();

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
