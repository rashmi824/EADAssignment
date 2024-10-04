using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Models
{
    public class Order
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string CustomerId { get; set; }
        public string VendorId { get; set; }
        public List<string> ProductIds { get; set; }
        public string Status { get; set; } // e.g., "Processing", "Delivered"
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public List<string> Notes { get; set; } // For cancellation notes
    }
}
