using Microsoft.AspNetCore.Mvc;
using OrderService.Core.Dto;
using OrderService.Core.Entities;
using OrderService.Core.Enums;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly Application.Services.OrderService _orderService;

    public OrderController(Application.Services.OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost("place-order/{orderId}")]
    public async Task<IActionResult> PlaceOrder([FromRoute] Guid orderId)
    {
        await _orderService.PlaceOrder(orderId);
        return Accepted();
    }

    [HttpGet("get-order-status/{orderId}")]
    public async Task<IActionResult> GetOrderStatus([FromRoute] Guid orderId)
    {
        var orderStatus = await _orderService.GetOrderStatus(orderId);
        return Ok(orderStatus);
    }

    [HttpGet("GetAllOrders")]
    public async Task<IActionResult> GetAllTask()
    {
        var orders = await _orderService.GetAllAsync();
        return Ok(orders);
    }

    [HttpGet("GetOrderById/{orderId}")]
    public async Task<IActionResult> GetOrderById([FromRoute] Guid orderId)
    {
        try
        {
            var order = await _orderService.GetByIdAsync(orderId);
            return Ok(order);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Order with ID {orderId} not found.");
        }
    }


    [HttpGet("GetActiveOrderByUserId")]
    public async Task<IActionResult> GetActiveOrderByUserId([FromQuery] int userId)
    {
        var orderItems = await _orderService.GetActiveOrderByUserId(userId);
        return Ok(orderItems);
    }

    [HttpPost("AddProductToOrder/{orderId}")]
    public async Task<IActionResult> AddProductToOrder([FromRoute] Guid orderId, [FromBody] AddProductToOrderDto request)
    {
        await _orderService.ReserveProductForOrder(orderId, request.ProductId, request.Quantity);
        return Accepted();
    }

    [HttpDelete("RemoveProductFromOrder/{orderId}/{productId}/{quantity}")]
    public async Task<IActionResult> RemoveProductFromOrder([FromRoute] Guid orderId, [FromRoute] int productId,
        [FromRoute] int quantity)
    {
        try
        {
            await _orderService.ReleaseReservationOfProduct(orderId, productId, quantity);
            return Ok();
        }
        catch (Exception ex)
        {
            if(ex is KeyNotFoundException)
                return BadRequest(ex.Message);
            if(ex is InvalidOperationException)
                return Conflict(ex.Message);
            return StatusCode(500, "An error occurred while removing the product from the order.");
        }
    }
}