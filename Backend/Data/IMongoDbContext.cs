/*
 IMongoDbContext.cs
 This interface defines the MongoDB collections for different entities in the system, such as users, vendors, and orders.
 */

using Backend.Models;
using MongoDB.Driver;

namespace Backend.Data
{
    // Interface for MongoDB context to manage collections
    public interface IMongoDbContext
    {
        // MongoDB collection for User entities
        IMongoCollection<User> Users { get; }

        // MongoDB collection for Vendor entities
        IMongoCollection<Vendor> Vendors { get; }

        // MongoDB collection for Comment entities
        IMongoCollection<Comment> Comments { get; }
        
        // MongoDB collection for Order entities
        IMongoCollection<Order> Orders { get; }
    }
}
