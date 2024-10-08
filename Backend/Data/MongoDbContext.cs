/*
 MongoDbContext.cs
 This class implements the IMongoDbContext interface and establishes a connection to the MongoDB 
 * database using settings from the configuration file. It provides access to the MongoDB collections 
 * for different entities such as Users, Vendors, Comments, and Orders.
 */

using Backend.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Backend.Data
{
    // Implementation of MongoDbContext which provides MongoDB collections and manages the database connection.
    public class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoDatabase _database; // Holds the MongoDB database instance

        // Constructor to initialize MongoDB context using settings from the configuration file
        public MongoDbContext(IConfiguration configuration)
        {
            // Retrieve MongoDB settings from appsettings.json (ConnectionString and DatabaseName)
            var mongoDbSettings = configuration.GetSection("MongoDbSettings");
            var connectionString = mongoDbSettings["ConnectionString"]; // Connection string to MongoDB
            var databaseName = mongoDbSettings["DatabaseName"]; // Database name

            // Initialize MongoDB client and database
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        // Property to access the Users collection in MongoDB
        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");

        // Property to access the Vendors collection in MongoDB
        public IMongoCollection<Vendor> Vendors => _database.GetCollection<Vendor>("Vendors");

        // Property to access the Comments collection in MongoDB
        public IMongoCollection<Comment> Comments => _database.GetCollection<Comment>("Comments");

        // Property to access the Orders collection in MongoDB
        public IMongoCollection<Order> Orders => _database.GetCollection<Order>("Order");
    }
}
