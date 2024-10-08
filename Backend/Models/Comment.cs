// Comment.cs
// This class represents a comment made by a customer regarding a vendor. 
// It includes details such as the customer's ID, the comment text, 
// and a rating associated with the comment. The rating is immutable after creation.

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Comment
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } // MongoDB will generate this automatically if null

    public required string CustomerId { get; set; } // ID of the customer making the comment

    public string? CommentText { get; set; } // Text of the comment

    public required double Rating { get; set; } // Immutable after creation
}
