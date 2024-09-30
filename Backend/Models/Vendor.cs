using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Vendor
{
    [BsonId] // This attribute tells MongoDB to use this property as the identifier
    [BsonRepresentation(BsonType.ObjectId)] // This specifies the type to be used in MongoDB
    public string? Id { get; set; } // MongoDB will automatically generate this ID if it's null
    public required string Name { get; set; }
    public double AverageRating { get; set; } = 0; // Initialize AverageRating if needed
    public List<Comment> Comments { get; set; } = new List<Comment>(); // Initialize the list to avoid null references
}
