using Microsoft.AspNetCore.Mvc;
using Backend.Dtos;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDto orderDto)
        {
            var order = await _orderService.CreateOrder(orderDto);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order); // Return ObjectId as string
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(string id)
        {
            var order = await _orderService.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(string id, [FromBody] OrderDto orderDto)
        {
            if (orderDto == null)
            {
                return BadRequest("Order data is required.");
            }

            var order = await _orderService.UpdateOrder(id, orderDto);
            if (order == null)
            {
                return NotFound("Order not found.");
            }
            return Ok(order);
        }

        [HttpPut("mark-delivered/{id}")]
        public async Task<IActionResult> MarkAsDelivered(string id)
        {
            var order = await _orderService.MarkAsDelivered(id);
            if (order == null)
            {
                return NotFound("Order not found.");
            }
            return Ok(order); // Return the updated order details
        }

        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelOrder(string id)
        {
            // Cancel the order and retrieve the updated order
            var canceledOrder = await _orderService.CancelOrder(id);
            
            if (canceledOrder == null)
            {
                return NotFound("Order not found.");
            }

            return Ok(canceledOrder); // Return the canceled order details
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            var isDeleted = await _orderService.DeleteOrder(id);
            if (!isDeleted)
            {
                return NotFound();
            }
            return NoContent(); // 204 No Content
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetOrdersByCustomerId(string customerId)
        {
            var orders = await _orderService.GetOrdersByCustomerId(customerId);
            return Ok(orders);
        }

        [HttpGet("vendor/{vendorId}")]
        public async Task<IActionResult> GetOrdersByVendorId(string vendorId)
        {
            var orders = await _orderService.GetOrdersByVendorId(vendorId);
            return Ok(orders);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrders();
            return Ok(orders);
        }
    }
}
