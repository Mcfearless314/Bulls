using Microsoft.EntityFrameworkCore;
using StockService.Application.Services;
using StockService.Core.Interfaces;
using StockService.Infrastructure;
using StockService.Infrastructure.Repositories;

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
