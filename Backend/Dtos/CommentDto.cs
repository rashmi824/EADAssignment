public class CommentDto
{
    public string CommentText { get; set; }
    public int Rating { get; set; } // Ensure this property exists

    public CommentDto(string commentText, int rating) // Updated constructor
    {
        CommentText = commentText;
        Rating = rating; // Initialize Rating
    }
}
