using Backend.Data;
using Backend.Models;
using Backend.Dtos;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class OrderService : IOrderService
{
    private readonly IMongoDbContext _context;
    private readonly GmailService _gmailService;

    public OrderService(IMongoDbContext context, GmailService gmailService)
    {
        _context = context;
        _gmailService = gmailService; // Use the parameter name without underscore
    }


    public async Task<Order> CreateOrder(OrderDto orderDto)
    {
        var order = new Order
        {
            CustomerId = orderDto.CustomerId,
            CustomerEmail = orderDto.CustomerEmail,
            VendorId = orderDto.VendorId,
            ProductIds = orderDto.ProductIds,
            Status = orderDto.Status,
            OrderDate = DateTime.UtcNow,
            Note = orderDto.Note,
            DeliveryDate = orderDto.DeliveryDate
        };

        order.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(); // Generate new ObjectId as string

        await _context.Orders.InsertOneAsync(order);
        
        return order;
    }
    public async Task<Order> UpdateOrder(string orderId, OrderDto orderDto)
        {
            if (string.IsNullOrWhiteSpace(orderId) || orderDto == null)
                throw new ArgumentNullException("Invalid parameters.");

            var filter = Builders<Order>.Filter.Eq(o => o.Id, orderId);
            var update = Builders<Order>.Update
                .Set(o => o.CustomerId, orderDto.CustomerId)
                .Set(o => o.CustomerEmail, orderDto.CustomerEmail)
                .Set(o => o.VendorId, orderDto.VendorId)
                .Set(o => o.ProductIds, orderDto.ProductIds)
                .Set(o => o.Status, orderDto.Status)
                .Set(o => o.DeliveryDate, orderDto.DeliveryDate)
                .Set(o => o.Note, orderDto.Note);

            var updateResult = await _context.Orders.UpdateOneAsync(filter, update);

            // Check if any document was modified
            if (updateResult.ModifiedCount == 0)
                return null; // No document was found or modified

            return await _context.Orders.Find(filter).FirstOrDefaultAsync();
        }

    public async Task<Order> CancelOrder(string orderId)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.Id, orderId);
        var update = Builders<Order>.Update.Set(o => o.Status, "Canceled");
        var result = await _context.Orders.UpdateOneAsync(filter, update);
        
        // Check if the order was modified
        if (result.ModifiedCount > 0)
        {
            // Return the updated order details
            return await _context.Orders.Find(filter).FirstOrDefaultAsync();
        }

        NotifyCustomerOrderStatus(orderId);
        
        return null; // Order not found or not modified
    }


    public async Task<Order> MarkAsDelivered(string orderId)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.Id, orderId);
        var update = Builders<Order>.Update
            .Set(o => o.Status, "Delivered")
            .Set(o => o.DeliveryDate, DateTime.UtcNow);

        // Update the order status and delivery date
        var result = await _context.Orders.UpdateOneAsync(filter, update);

        // Check if any document was modified
        if (result.ModifiedCount > 0)
        {

            NotifyCustomerOrderStatus(orderId);
            // Return the updated order details
            return await _context.Orders.Find(filter).FirstOrDefaultAsync();

          
        }

        return null; // Return null if the order was not found or not modified
    }

    public async Task<Order> GetOrderById(string orderId)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.Id, orderId);
        return await _context.Orders.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<List<Order>> GetOrdersByCustomerId(string customerId)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            throw new ArgumentNullException(nameof(customerId));

        return await _context.Orders.Find(o => o.CustomerId == customerId).ToListAsync();
    }

    public async Task<List<Order>> GetOrdersByVendorId(string vendorId)
    {
        if (string.IsNullOrWhiteSpace(vendorId))
            throw new ArgumentNullException(nameof(vendorId));

        return await _context.Orders.Find(o => o.VendorId == vendorId).ToListAsync();
    }

    public async Task<List<Order>> GetAllOrders()
    {
        return await _context.Orders.Find(_ => true).ToListAsync();
    }

    public async Task<bool> DeleteOrder(string orderId)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.Id, orderId);
        var result = await _context.Orders.DeleteOneAsync(filter);
        return result.DeletedCount > 0; // Returns true if the order was deleted
    }

    public async Task NotifyCustomerOrderStatus(string orderId)
{
    // Retrieve the order details
    var order = await _context.Orders.Find(o => o.Id == orderId).FirstOrDefaultAsync();
    if (order == null)
    {
        Console.WriteLine($"Order with ID {orderId} not found.");
        return;
    }

    // Prepare the email details
    string subject = "Order Status Update";
    string message = $"Hello,\n\nYour order with ID: {order.Id} is now marked as {order.Status}.\n\nThank you for your Order!\n\nBest Regards,\nStyle Hevan";

    // Send the notification
    try
    {
        await _gmailService.SendEmailAsync(order.CustomerEmail, subject, message);
        Console.WriteLine($"Email successfully sent to {order.CustomerEmail}.");
    }
    catch (Exception ex)
    {
        // Log the error (you can implement a logger or handle the exception)
        Console.WriteLine($"Error sending email to {order.CustomerEmail}: {ex.Message}");
    }
}

}
