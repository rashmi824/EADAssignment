using Backend.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IOrderService
{
    Task<List<Order>> GetOrdersAsync();
    Task<Order> GetOrderByIdAsync(string id);
    Task<Order> CreateOrderAsync(Order newOrder);
    Task<bool> UpdateOrderAsync(string id, Order updatedOrder);
    Task<bool> DeleteOrderAsync(string id);
}
