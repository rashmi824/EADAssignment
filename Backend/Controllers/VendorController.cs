using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]

[ApiController]
public class VendorController : ControllerBase
{
    private readonly IUserService _userService; // Assuming UserService handles vendor management

    public VendorController(IUserService userService)
    {
        _userService = userService;
    }

    // Endpoint to add a comment and update the vendor's average rating
    [HttpPost("add-comment/{vendorId}")]
    [Authorize(Roles = "Customer")] // Ensure only users with the "Customer" role can access this endpoint
    public async Task<IActionResult> AddComment(string vendorId, [FromBody] CommentDto commentDto)
    {
        // Validate the incoming data
        if (commentDto == null || string.IsNullOrWhiteSpace(commentDto.CustomerId) || 
            string.IsNullOrWhiteSpace(commentDto.CommentText) || commentDto.Rating < 0 || commentDto.Rating > 5)
        {
            return BadRequest("Invalid comment data.");
        }

        // Construct the comment from the DTO
        var comment = new Comment
        {
            CustomerId = commentDto.CustomerId,
            CommentText = commentDto.CommentText,
            Rating = commentDto.Rating
        };

        // Call the service method to add the comment and update the average rating
        var updatedVendor = await _userService.AddCommentAndUpdateRating(vendorId, comment);

        if (updatedVendor != null)
        {
            return Ok(new 
            { 
                Message = "Comment added and vendor's average rating updated.", 
                Vendor = updatedVendor // Return the updated vendor data
            });
        }

        return NotFound("Vendor not found or the operation failed.");
    }

    [HttpPut("update-comment/{vendorId}/{commentId}")]
    [Authorize(Roles = "Customer")] // Ensure only users with the "Customer" role can access this endpoint
    public async Task<IActionResult> UpdateComment(string vendorId, string commentId, [FromBody] string commentText)
    {
        // Call the service method to update the comment
        var updatedUser = await _userService.UpdateComment(vendorId, commentId, commentText);

        if (updatedUser != null)
        {
            return Ok(updatedUser); // Return the updated user
        }

        return NotFound("Vendor or comment not found.");
    }

}
