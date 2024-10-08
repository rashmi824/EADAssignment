using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Backend.Models
{
    public class Inventory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ID { get; set; }

        [BsonElement("productID")]
        public string ProductID { get; set; }

        [BsonElement("productname")]
        public string ProductName { get; set; }

        [BsonElement("stockLevel")]
        public int StockLevel { get; set; }

        [BsonElement("stockStatus")]
        public string StockStatus { get; set; }
    }
}
