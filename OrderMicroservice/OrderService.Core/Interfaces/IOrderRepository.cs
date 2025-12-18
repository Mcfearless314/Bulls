using OrderService.Core.Entities;
using OrderService.Core.Enums;

namespace OrderService.Core.Interfaces;

public interface IOrderRepository
{
    public Task<IEnumerable<Order>> GetAllAsync();
    public Task<Order> GetByIdAsync(Guid id);
    public Task<Order> CreateAsync(Order order);
    public Task<Order> UpdateAsync(Guid id, OrderStatus status);
    public Task<Order> DeleteAsync(Guid id);
    public Task AddItemToOrder(Guid orderId, int productId, string productName, double price, int quantity);
    public Task DeleteOrderItemFromOrder(Guid orderId, int productId, int quantity);
    public Task<Order?> GetActiveOrderByUserId(int i);
}