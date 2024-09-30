using MongoDB.Driver;


namespace Backend.Data
{
    public interface IMongoDbContext
    {
        IMongoCollection<User> Users { get; }
        IMongoCollection<Vendor> Vendors { get; }
        // IMongoCollection<Product> Products { get; }
        IMongoCollection<Comment> Comments { get; }

        IMongoCollection<Order> Orders { get; }
    }
}
