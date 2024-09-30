using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Backend.Data;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // GET: api/orders
    [HttpGet]
    public async Task<ActionResult<List<Order>>> GetOrders()
    {
        var orders = await _orderService.GetOrdersAsync();
        return Ok(orders);
    }

    // GET: api/orders/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrderById(string id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
        {
            return NotFound();
        }
        return Ok(order);
    }

    // POST: api/orders

    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(Order newOrder)
    {
        var order = await _orderService.CreateOrderAsync(newOrder);
        return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
    }


    // PUT: api/orders/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(string id, [FromBody] Order updatedOrder)
    {
        var existingOrder = await _orderService.GetOrderByIdAsync(id);
        if (existingOrder == null)
        {
            return NotFound();
        }

        updatedOrder.Id = existingOrder.Id; // Ensure ID remains the same
        var result = await _orderService.UpdateOrderAsync(id, updatedOrder);
        if (!result)
        {
            return StatusCode(500, "Failed to update order");
        }
        return NoContent();
    }

    // DELETE: api/orders/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(string id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        var result = await _orderService.DeleteOrderAsync(id);
        if (!result)
        {
            return StatusCode(500, "Failed to delete order");
        }
        return NoContent();
    }
}
