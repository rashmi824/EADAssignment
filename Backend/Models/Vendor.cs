// Vendor.cs
// This class represents a vendor in the system, including their average rating and a collection of comments 
// from users. It is used to manage vendor-related information and customer feedback.

public class Vendor
{
    public double AverageRating { get; set; } = 0; // Initialize AverageRating if needed
    public List<Comment> Comments { get; set; } = new List<Comment>(); // Initialize the list to avoid null references
}
