using OrderService.Core.Entities;
using OrderService.Core.Enums;

namespace OrderService.Core.Interfaces;

public interface IOrderRepository
{
    public Task<IEnumerable<Order>> GetAllAsync();
    public Task<Order> GetByIdAsync(int id);
    public Task<Order> CreateAsync(Order order);
    public Task<Order> UpdateAsync(int id, OrderStatus status);
    public Task<Order> DeleteAsync(int id);
    public Task AddItemToOrder(int orderId, int productId, string productName, decimal price, int quantity);
    public Task DeleteOrderItemFromOrder(int orderId, int productId, int quantity);
}