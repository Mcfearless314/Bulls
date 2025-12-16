using OrderService.Core.DomainEvents;
using OrderService.Core.Entities;
using OrderService.Core.Enums;
using OrderService.Core.Interfaces;

namespace OrderService.Application.Services;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMessageClient _messageClient;

    public OrderService(IOrderRepository orderRepository, IMessageClient messageClient)
    {
        _orderRepository = orderRepository;
        _messageClient = messageClient;
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _orderRepository.GetAllAsync();
    }

    public async Task<Order> GetByIdAsync(int id)
    {
        return await _orderRepository.GetByIdAsync(id);
    }

    public async Task<Order> CreateAsync(Order order)
    {
        return await _orderRepository.CreateAsync(order);
    }

    public void UpdateOrderStatus(int orderId, OrderStatus orderStatus)
    {
        
    }

    public async Task<Order> DeleteAsync(int id)
    {
        return await _orderRepository.DeleteAsync(id);
    }

    public async Task AddItemToOrderTask(int orderId, int productId, string productName, decimal price, int quantity)
    {
        await _orderRepository.AddItemToOrder(orderId, productId, productName, price, quantity);
    }

    public async Task ReleaseReservationOfProduct(int orderId, int productId, int quantity)
    {
       await _orderRepository.DeleteOrderItemFromOrder(orderId, productId, quantity);
       
       await _messageClient.PublishAsync(new ProductRemovedFromOrderItems
       {
           OrderId = orderId,
           ProductId = productId,
           Quantity = quantity
       });
    }
}