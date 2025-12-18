using OrderService.Core.DomainEvents;
using OrderService.Core.Entities;
using OrderService.Core.Enums;
using OrderService.Core.Exchanges;
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

    public async Task UpdateOrderStatus(Guid orderId, OrderStatus orderStatus)
    {
        await _orderRepository.UpdateAsync(orderId, orderStatus);
    }

    public async Task<Order> DeleteAsync(Guid id)
    {
        return await _orderRepository.DeleteAsync(id);
    }


    public async Task ReleaseReservationOfProduct(Guid orderId, int productId, int quantity)
    {
        var order = await GetByIdAsync(orderId);
        if (order.Status != OrderStatus.Initialized)
        {
            throw new InvalidOperationException("Order is not in a valid state to place an order.");
        }

        await _orderRepository.DeleteOrderItemFromOrder(orderId, productId, quantity);

        var removedProductFromOrderItems = new RemovedProductFromOrderItems
        {
            OrderId = order.Id,
            ProductId = productId,
            Quantity = quantity
        };

        await _messageClient.PublishAsync(removedProductFromOrderItems, OrderEvent.RemovedProductFromOrderItems);
    }

    public async Task<Order?> GetActiveOrderByUserId(int id)
    {
        return await _orderRepository.GetActiveOrderByUserId(id);
    }

    public async Task ReserveProductForOrder(Guid orderId, int requestProductId, int requestQuantity)
    {
        try
        {
            var order = await GetByIdAsync(orderId);
            if (order.Status != OrderStatus.Initialized)
            {
                throw new InvalidOperationException("Order is not in a valid state to place an order.");
            }

            var productAddedToOrder = new ProductAddedToOrder
            {
                OrderId = order.Id,
                ProductId = requestProductId,
                Quantity = requestQuantity
            };

            await _messageClient.PublishAsync(productAddedToOrder, OrderEvent.ProductAddedToOrder);
        }
        catch (Exception ex)
        {
            var productAddedToOrderFailed = new ProductAddedToOrderFailed
            {
                OrderId = orderId,
                ProductId = requestProductId,
                Quantity = requestQuantity,
                Reason = ex.Message
            };
            await _messageClient.PublishAsync(productAddedToOrderFailed, OrderEvent.OrderPlacingFailed);
        }
    }

    public async Task AddItemToOrderTask(Guid orderId, int productId, string productName, decimal price, int quantity)
    {
        await _orderRepository.AddItemToOrder(orderId, productId, productName, price, quantity);
    }

    public object? CheckoutOrder(Guid orderId)
    {
        throw new NotImplementedException();
    }

    public object? CancelOrder(Guid orderId)
    {
        throw new NotImplementedException();
    }

    public async Task PlaceOrder(Guid orderId)
    {
        try
        {
            var order = await GetByIdAsync(orderId);
            if (order.Status == OrderStatus.Confirmed)
            {
                Console.WriteLine("Order has already been confirmed");
                return;
            }
            
            await UpdateOrderStatus(orderId, OrderStatus.PendingConfirmation);
            var orderPlaced = new OrderPlaced
            {
                OrderId = order.Id,
                UserId = order.UserId,
                ProductsAndQuantities = order.Items
                    .ToDictionary(
                        item => item.ProductId,
                        item => item.Quantity
                    ),
                Price = order.Items.Sum(item => item.Price * item.Quantity)
            };
            await _messageClient.PublishAsync(orderPlaced, OrderEvent.OrderPlaced);
        }
        catch (Exception ex)
        {
            var orderFailed = new OrderPlacingFailed
            {
                OrderId = orderId,
                Reason = ex.Message
            };
            await _messageClient.PublishAsync(orderFailed, OrderEvent.OrderPlacingFailed);
        }
    }

    public async Task<string> GetOrderStatus(Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        return order.Status.ToString();
    }
}