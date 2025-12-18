using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using AuthenticationService;
using AuthenticationService.Application.Services;
using AuthenticationService.Core.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration.AddJsonFile("ocelot.json", false, false);

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
while (string.IsNullOrEmpty(secretSettings.BullsToken))
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

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretSettings.BullsToken))
    };
});
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseOcelot().Wait();

app.UseHttpsRedirection();

app.Run();
