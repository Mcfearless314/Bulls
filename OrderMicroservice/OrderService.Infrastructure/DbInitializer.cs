using OrderService.Core.Entities;
using OrderService.Core.Enums;

namespace OrderService.Infrastructure;

public class DbInitializer
{
    public async Task InitializeAsync(AppDbContext context)
    {
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        var orders = new List<Order>();

        for (int i = 1; i <= 10; i++)
        {
            orders.Add(new Order
            {
                Id = Guid.NewGuid(),
                UserId = 1,
                CreatedAt = DateTime.Now,
                Status = OrderStatus.Initialized
            });
        }

        await context.Orders.AddRangeAsync(orders);
        await context.SaveChangesAsync();
    }
}