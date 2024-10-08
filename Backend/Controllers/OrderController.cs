// Controller for handling order-related operations such as creating, updating, deleting, and retrieving orders. 
// Supports actions for both customers and vendors.
using Microsoft.AspNetCore.Mvc;
using Backend.Dtos;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/orders")]
    // Constructor to initialize OrderController with the order service dependency.
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        // Creates a new order
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDto orderDto)
        {
            var order = await _orderService.CreateOrder(orderDto);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order); // Return ObjectId as string
        }
        // Retrieves an order by its unique ID.
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
        // Updates an existing order based on the given ID.
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
        // Marks an order as delivered based on the given ID.
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
        // Cancels an existing order by its ID.
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

        // Deletes an order by its ID.
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
        // Retrieves all orders placed by a specific customer.
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetOrdersByCustomerId(string customerId)
        {
            var orders = await _orderService.GetOrdersByCustomerId(customerId);
            return Ok(orders);
        }
        // Retrieves all orders handled by a specific vendor.
        [HttpGet("vendor/{vendorId}")]
        public async Task<IActionResult> GetOrdersByVendorId(string vendorId)
        {
            var orders = await _orderService.GetOrdersByVendorId(vendorId);
            return Ok(orders);
        }
        // Retrieves all orders in the system.
        // Returns a list of all orders.
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrders();
            return Ok(orders);
        }
    }
}
