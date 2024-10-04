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
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id.ToString() }, order); // Return ObjectId as string
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
            var order = await _orderService.UpdateOrder(id, orderDto);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelOrder(string id, [FromBody] string note)
        {
            var result = await _orderService.CancelOrder(id, note);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPut("mark-delivered/{id}")]
        public async Task<IActionResult> MarkAsDelivered(string id)
        {
            var order = await _orderService.MarkAsDelivered(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
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
