
public class Comment
{
    public required string CustomerId { get; set; }
    public string? CommentText { get; set; }
    public required double Rating { get; set; } // Immutable after creation
}
