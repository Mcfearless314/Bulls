using OrderService.Core.Entities;
using OrderService.Core.Interfaces;

namespace OrderService.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    public Task<IEnumerable<Order>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Order> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Order> CreateAsync(Order order)
    {
        throw new NotImplementedException();
    }

    public Task<Order> UpdateAsync(Order order)
    {
        throw new NotImplementedException();
    }

    public Task<Order> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}