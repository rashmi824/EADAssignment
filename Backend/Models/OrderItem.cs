// Represents an individual item within an order, containing details about the product, quantity, and price.

public class OrderItem
{
    //  Gets or sets the ID of the product being ordered.
    public string ProductId { get; set; }

    // Gets or sets the name of the product being ordered.
    public string ProductName { get; set; }

    // Gets or sets the quantity of the product being ordered.
    public int Quantity { get; set; }

    // Gets or sets the price of the product. This represents the cost of a single unit of the product.
    public decimal Price { get; set; } // Price information for each item
}