using OrderService.Core.Entities;
using OrderService.Core.Enums;

namespace OrderService.Infrastructure;

public class DbInitializer
{
    public async Task InitializeAsync(AppDbContext context)
    {
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = 1,
            Status = OrderStatus.Initialized,
            CreatedAt = DateTime.Now,
        };
        
        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();

        var orderItems = new List<OrderItem>
        {
            new OrderItem
            {
                OrderId = order.Id,
                ProductId = 1,
                ProductName = "Screen",
                Quantity = 2,
                Price = 1000,
            },
            new OrderItem
            {
                OrderId = order.Id,
                ProductId = 3,
                ProductName = "Graphics Card",
                Quantity = 1,
                Price = 5000,
            }
        };
        
        await context.OrderItems.AddRangeAsync(orderItems);
        await context.SaveChangesAsync();
    }
}