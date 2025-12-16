using Microsoft.AspNetCore.Mvc;

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
    
    
    
    [HttpDelete("removeProductFromOrder/{orderId}/{productId}/{quantity}")]
    public IActionResult RemoveProductFromOrder([FromRoute] int orderId, [FromRoute] int productId, [FromRoute] int quantity)
    {
        return Ok(_orderService.ReleaseReservationOfProduct(orderId, productId, quantity));
    }
}