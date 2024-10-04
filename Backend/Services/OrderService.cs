using Backend.Data;
using Backend.Models;
using Backend.Dtos;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class OrderService : IOrderService
{
    private readonly IMongoDbContext _context;

    public OrderService(IMongoDbContext context)
    {
        _context = context;
    }

    public async Task<Order> CreateOrder(OrderDto orderDto)
    {
        var order = new Order
        {
            CustomerId = orderDto.CustomerId,
            VendorId = orderDto.VendorId,
            ProductIds = orderDto.ProductIds,
            Status = orderDto.Status,
            OrderDate = DateTime.UtcNow,
            Notes = new List<string>()
        };

        await _context.Orders.InsertOneAsync(order);
        return order;
    }

    public async Task<Order> UpdateOrder(string orderId, OrderDto orderDto)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.Id, new ObjectId(orderId));
        var update = Builders<Order>.Update
            .Set(o => o.VendorId, orderDto.VendorId)
            .Set(o => o.ProductIds, orderDto.ProductIds)
            .Set(o => o.Status, orderDto.Status);

        await _context.Orders.UpdateOneAsync(filter, update);
        return await _context.Orders.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<bool> CancelOrder(string orderId, string note)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.Id, new ObjectId(orderId));
        var update = Builders<Order>.Update.AddToSet(o => o.Notes, note);
        var result = await _context.Orders.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    public async Task<Order> MarkAsDelivered(string orderId)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.Id, new ObjectId(orderId));
        var update = Builders<Order>.Update
            .Set(o => o.Status, "Delivered")
            .Set(o => o.DeliveryDate, DateTime.UtcNow);

        await _context.Orders.UpdateOneAsync(filter, update);
        return await _context.Orders.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<Order> GetOrderById(string orderId)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.Id, new ObjectId(orderId));
        return await _context.Orders.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<List<Order>> GetOrdersByCustomerId(string customerId)
    {
        return await _context.Orders.Find(o => o.CustomerId == customerId).ToListAsync();
    }

    public async Task<List<Order>> GetOrdersByVendorId(string vendorId)
    {
        return await _context.Orders.Find(o => o.VendorId == vendorId).ToListAsync();
    }

    public async Task<List<Order>> GetAllOrders()
    {
        return await _context.Orders.Find(_ => true).ToListAsync(); // Fetch all orders
    }
}
