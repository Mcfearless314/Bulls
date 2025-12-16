using Microsoft.EntityFrameworkCore;
using OrderService.Core.Entities;
using OrderService.Core.Enums;
using OrderService.Core.Interfaces;

namespace OrderService.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _context.Orders
            .Include(o => o.Items)
            .ToListAsync();
    }

    public async Task<Order> GetByIdAsync(Guid id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            Console.WriteLine("Order does not exist");
            throw new KeyNotFoundException("Order not found for the given OrderId");
        }

        return order;
    }

    public async Task<Order> CreateAsync(Order order)
    {
        var createdOrder = await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
        return createdOrder.Entity;
    }

    public async Task<Order> UpdateAsync(Guid id, OrderStatus status)
    {
        var order = await GetByIdAsync(id);
        order.Status = status;
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Order> DeleteAsync(Guid id)
    {
        var deletedOrder = await _context.Orders.FindAsync(id);
        _context.Orders.Remove(deletedOrder);
        await _context.SaveChangesAsync();
        return deletedOrder;
    }

    public async Task AddItemToOrder(Guid orderId, int productId, string productName, decimal price, int quantity)
    {
        var order = await GetByIdAsync(orderId);

        order.Items.Add(new OrderItem
        {
            OrderId = orderId,
            ProductId = productId,
            ProductName = productName,
            Price = price,
            Quantity = quantity
        });
    }

    public async Task DeleteOrderItemFromOrder(Guid orderId, int productId, int quantity)
    {
        var order = await GetByIdAsync(orderId);
        var orderItem = order.Items.FirstOrDefault(oi => oi.ProductId == productId);

        if (orderItem != null)
        {
            orderItem.Quantity -= quantity;
            if (orderItem.Quantity <= 0)
            {
                order.Items.Remove(orderItem);
            }
        }
    }
}