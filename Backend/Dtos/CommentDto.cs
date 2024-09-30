public class CommentDto
{
    public string CustomerId { get; set; } // ID of the customer leaving the comment
    public string? CommentText { get; set; } // Optional text for the comment
    public double Rating { get; set; } // Rating (required)
}
