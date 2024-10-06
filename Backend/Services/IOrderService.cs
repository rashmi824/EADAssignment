using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Models;
using Backend.Dtos;

public interface IOrderService
{
    Task<Order> CreateOrder(OrderDto orderDto);
    Task<Order> UpdateOrder(string orderId, OrderDto orderDto);
    Task<Order> MarkAsDelivered(string orderId);
    Task<List<Order>> GetOrdersByCustomerId(string customerId);
    Task<List<Order>> GetOrdersByVendorId(string vendorId);
    // Add other methods as necessary
    Task<Order> GetOrderById(string orderId); // Newly added method

    Task<List<Order>> GetAllOrders();

    Task<Order> CancelOrder(string orderId);

    Task<bool> DeleteOrder(string orderId);
}
