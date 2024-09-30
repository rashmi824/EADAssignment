// using MongoDB.Bson;
// using MongoDB.Bson.Serialization.Attributes;
// using System;
// using System.Collections.Generic;

// namespace OrderManagementSystem.Models
// {
//     public class Order
//     {
//         [BsonId]
//         public ObjectId Id { get; set; }

//         public string CustomerId { get; set; }
//         public List<OrderItem> Items { get; set; }
//         public string Status { get; set; }
//         public DateTime OrderDate { get; set; }
//         public DateTime? DispatchDate { get; set; }
//         public string VendorId { get; set; }
//         public string Notes { get; set; }
//         public bool IsDelivered { get; set; }
//     }

//     public class OrderItem
//     {
//         public string ProductId { get; set; }
//         public int Quantity { get; set; }
//         public decimal Price { get; set; }
//     }
// }

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string CustomerId { get; set; } // New: Track customer information
    public string Status { get; set; }
    public string Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DispatchDate { get; set; } // New: Optional dispatch date
    public List<OrderItem> Items { get; set; }

    public Order()
    {
        CreatedAt = DateTime.UtcNow; // Automatically set created date
    }
}