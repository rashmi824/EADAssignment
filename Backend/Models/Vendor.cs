

public class Vendor
{

    public double AverageRating { get; set; } = 0; // Initialize AverageRating if needed
    public List<Comment> Comments { get; set; } = new List<Comment>(); // Initialize the list to avoid null references
}
