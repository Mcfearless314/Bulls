using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using AuthenticationService;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration.AddJsonFile("ocelot.json", false, false);

var vaultHostName = builder.Configuration.GetValue<string>("Settings:VaultHostName")
                    ?? throw new InvalidOperationException("Configuration key 'Settings:VaultHostName' is missing.");

var secretSettings = new SecretSettings();

while (string.IsNullOrEmpty(secretSettings.BullsToken))
{
    try
    {
        secretSettings = VaultHelper.FetchSecretsFromVault(vaultHostName, "bulls-user", "user"); // This would be entered securely in a real scenario (UI and hashing)
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
