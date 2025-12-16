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

    public async Task<Order> GetByIdAsync(Guid id)
    {
        return await _orderRepository.GetByIdAsync(id);
    }

    public async Task<Order> CreateAsync(Order order)
    {
        return await _orderRepository.CreateAsync(order);
    }

    public void UpdateOrderStatus(Guid orderId, OrderStatus orderStatus)
    {
        
    }

    public async Task<Order> DeleteAsync(Guid id)
    {
        return await _orderRepository.DeleteAsync(id);
    }

    public async Task AddItemToOrderTask(Guid orderId, int productId, string productName, decimal price, int quantity)
    {
        await _orderRepository.AddItemToOrder(orderId, productId, productName, price, quantity);
    }

    public async Task ReleaseReservationOfProduct(Guid orderId, int productId, int quantity)
    {
       await _orderRepository.DeleteOrderItemFromOrder(orderId, productId, quantity);
       
       await _messageClient.PublishAsync(new ProductRemovedFromOrderItems
       {
           OrderId = orderId,
           ProductId = productId,
           Quantity = quantity
       });
    }

    public async Task<Order?> GetActiveOrderByUserId(int i)
    {
        return await _orderRepository.GetActiveOrderByUserId(i);
    }

    public void ReserveProductForOrder(Guid orderId, int requestProductId, int requestQuantity)
    {
        throw new NotImplementedException();
    }
    
    public async Task<Order> GetActiveOrderByUserId(int id)
    {
        throw new  NotImplementedException();
    }

    public object? CheckoutOrder(Guid orderId)
    {
        throw new NotImplementedException();
    }

    public object? CancelOrder(Guid orderId)
    {
        throw new NotImplementedException();
    }
}