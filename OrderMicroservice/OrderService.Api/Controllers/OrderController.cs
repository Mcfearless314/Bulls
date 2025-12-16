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


    [HttpPost("CreateOrder")]
    public async Task<IActionResult> CreateOrder()
    {
        var order = await _orderService.GetActiveOrderByUserId(1); //TODO skal ændres til den rigtige bruger
        if (order != null) return Ok(order);

        /*order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = 1, //TODO skal ændres til den rigtige bruger
            Status = 0,
            CreatedAt = DateTime.Now,
            Items = new List<OrderItem>()
        };*/

        return Ok(order);
    }

    [HttpGet("GetActiveOrderByUserId")]
    public IActionResult GetActiveOrderByUserId([FromQuery] int userId)
    {
        var orderItems = _orderService.GetActiveOrderByUserId(userId);
        return Ok(orderItems);
    }

    [HttpPost("AddProductToOrder/{orderId}")]
    public IActionResult AddProductToOrder([FromRoute] Guid orderId, [FromBody] AddProductToOrderDto request)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("RemoveProductFromOrder/{orderId}/{productId}/{quantity}")]
    public IActionResult RemoveProductFromOrder([FromRoute] Guid orderId, [FromRoute] int productId,
        [FromRoute] int quantity)
    {
        return Ok(_orderService.ReleaseReservationOfProduct(orderId, productId, quantity));
    }

    [HttpPost("CheckoutOrder/{orderId}")]
    public IActionResult CheckoutOrder([FromRoute] Guid orderId)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("CancelOrder/{orderId}")]
    public IActionResult CancelOrder([FromRoute] Guid orderId)
    {
        throw new NotImplementedException();
    }
}