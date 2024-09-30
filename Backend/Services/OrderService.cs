using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using Backend.Data;

public class OrderService : IOrderService
{
    private readonly IMongoCollection<Order> _orders;

    public OrderService(IMongoDbContext dbContext)
    {
        _orders = dbContext.Orders;
    }

    public async Task<List<Order>> GetOrdersAsync()
    {
        return await _orders.Find(order => true).ToListAsync();
    }

    public async Task<Order> GetOrderByIdAsync(string id)
    {
        return await _orders.Find(order => order.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Order> CreateOrderAsync(Order newOrder)
    {
        // Generate a new ObjectId
        newOrder.Id = ObjectId.GenerateNewId().ToString(); // Convert ObjectId to string
        await _orders.InsertOneAsync(newOrder);
        return newOrder;
    }

    public async Task<bool> UpdateOrderAsync(string id, Order updatedOrder)
    {
        var result = await _orders.ReplaceOneAsync(order => order.Id == id, updatedOrder);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteOrderAsync(string id)
    {
        var result = await _orders.DeleteOneAsync(order => order.Id == id);
        return result.DeletedCount > 0;
    }
}
