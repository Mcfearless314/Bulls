using OrderService.Core.Entities;

namespace OrderService.Core.Interfaces;

public interface IOrderRepository
{
    public Task<IEnumerable<Order>> GetAllAsync();
    public Task<Order> GetByIdAsync(int id);
    public Task<Order> CreateAsync(Order order);
    public Task<Order> UpdateAsync(Order order);
    public Task<Order> DeleteAsync(int id);
}