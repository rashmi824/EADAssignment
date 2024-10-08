// Defines the contract for order-related operations in the application.
// The IOrderService interface provides methods for creating, updating, retrieving, 
// and managing orders.

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

    Task<Order> GetOrderById(string orderId);

    Task<List<Order>> GetAllOrders();

    Task<Order> CancelOrder(string orderId);

    Task<bool> DeleteOrder(string orderId);
}
