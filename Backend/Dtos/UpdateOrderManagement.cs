using System;

namespace Backend.Dtos
{
    public class OrderDto
    {
        public string CustomerId { get; set; }

        public string  CustomerEmail  { get; set; }
        public string VendorId { get; set; }
        public List<string> ProductIds { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? Note { get; set; }
    }
}
