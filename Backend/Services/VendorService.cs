using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Backend.Data;

public class VendorService : IVendorService
{
    private readonly IMongoCollection<Vendor> _vendors;

    public VendorService(IMongoDbContext dbContext)
    {
        _vendors = dbContext.Vendors; // Assuming you have a collection for vendors in your MongoDB context
    }
    public async Task<Vendor> CreateVendor(string name)
    {
        var vendor = new Vendor { Name = name }; // No need to set Id
        await _vendors.InsertOneAsync(vendor);
        return vendor; // MongoDB will set the Id automatically
    }

    public async Task AddComment(string vendorId, string customerId, string commentText, double rating)
    {
        var update = Builders<Vendor>.Update.Push(v => v.Comments, new Comment
        {
            CustomerId = customerId,
            CommentText = commentText,
            Rating = rating
        });

        // Update vendor with the new comment
        var updateResult = await _vendors.UpdateOneAsync(v => v.Id == vendorId, update);
        
        // Optionally, you can recalculate the average rating
        if (updateResult.ModifiedCount > 0)
        {
            var vendor = await _vendors.Find(v => v.Id == vendorId).FirstOrDefaultAsync();
            if (vendor != null)
            {
                vendor.AverageRating = vendor.Comments.Count > 0 
                    ? vendor.Comments.Average(c => c.Rating) 
                    : 0;
                
                var replaceResult = await _vendors.ReplaceOneAsync(v => v.Id == vendorId, vendor);
            }
        }
    }

    public async Task<IReadOnlyList<Vendor>> GetVendors() // Use IReadOnlyList for better encapsulation
    {
        return await _vendors.Find(v => true).ToListAsync(); // Return all vendors
    }
}
