// Represents an order in the system, stored in the MongoDB database.
// This model includes details about the customer, vendor, products, and order status.
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string CustomerId { get; set; }

        public string CustomerEmail { get; set; }
        public string VendorId { get; set; }
        public List<string> ProductIds { get; set; }
        public string Status { get; set; } // e.g., "Processing", "Delivered"
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        // Notes for delivery instructions, such as "pack as gift"
        public string? Note { get; set; }
    }
}
