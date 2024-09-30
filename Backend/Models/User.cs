using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } // MongoDB will generate this automatically if null

    public required string Email { get; set; } // Email is the primary identifier

    public required string Username { get; set; } 

    public required int MobileNumber { get; set; }

    public required string Address { get; set; }

    public required string PasswordHash { get; set; }

    public required string Role { get; set; } // Administrator, Vendor, CSR, or Customer

    public bool? IsApproved { get; set; } // Null for non-Customer roles, false for unapproved customers

    public Vendor? VendorDetails { get; set; } // Nested Vendor details, null for non-Vendor users
}
