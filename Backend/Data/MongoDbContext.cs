using Backend.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;


namespace Backend.Data
{
    public class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            // Retrieve the MongoDB settings from appsettings.json
            var mongoDbSettings = configuration.GetSection("MongoDbSettings");
            var connectionString = mongoDbSettings["ConnectionString"];
            var databaseName = mongoDbSettings["DatabaseName"];

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
        public IMongoCollection<Vendor> Vendors => _database.GetCollection<Vendor>("Vendors");
        //public IMongoCollection<Product> Products => _database.GetCollection<Product>("Products");
        public IMongoCollection<Comment> Comments => _database.GetCollection<Comment>("Comments");
        //public IMongoCollection<Order> Orders => _database.GetCollection<Order>("Orders");
        public IMongoCollection<Order> Orders => _database.GetCollection<Order>("Orders");
    }
}
