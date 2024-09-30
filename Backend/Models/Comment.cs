using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Comment

{

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } // MongoDB will generate this automatically if null
    public required string CustomerId { get; set; }
    public string? CommentText { get; set; }
    public required double Rating { get; set; } // Immutable after creation
}
