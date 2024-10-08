/*
 File: VendorController.cs
 This file contains endpoints for managing vendor comments and updating vendor ratings.
 */

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
public class VendorController : ControllerBase
{
    private readonly IUserService _userService; // Service to handle vendor and comment management

    // Constructor to initialize the UserService for vendor-related operations
    public VendorController(IUserService userService)
    {
        _userService = userService;
    }

    // Endpoint to add a comment and update the vendor's average rating
    [HttpPost("add-comment/{vendorId}")]
    [Authorize(Roles = "Customer")] // Restricts access to users with the "Customer" role
    public async Task<IActionResult> AddComment(string vendorId, [FromBody] CommentDto commentDto)
    {
        // Validate the incoming data (comment text, customer ID, rating)
        if (commentDto == null || string.IsNullOrWhiteSpace(commentDto.CustomerId) || 
            string.IsNullOrWhiteSpace(commentDto.CommentText) || commentDto.Rating < 0 || commentDto.Rating > 5)
        {
            return BadRequest("Invalid comment data.");
        }

        // Create a new comment from the DTO
        var comment = new Comment
        {
            CustomerId = commentDto.CustomerId,
            CommentText = commentDto.CommentText,
            Rating = commentDto.Rating
        };

        // Call the service to add the comment and update the vendor's rating
        var updatedVendor = await _userService.AddCommentAndUpdateRating(vendorId, comment);

        // Return success response if vendor was updated
        if (updatedVendor != null)
        {
            return Ok(new 
            { 
                Message = "Comment added and vendor's average rating updated.", 
                Vendor = updatedVendor // Include the updated vendor data
            });
        }

        return NotFound("Vendor not found or the operation failed.");
    }

    // Endpoint to update an existing comment by its ID
    [HttpPut("update-comment/{vendorId}/{commentId}")]
    [Authorize(Roles = "Customer")] // Restricts access to users with the "Customer" role
    public async Task<IActionResult> UpdateComment(string vendorId, string commentId, [FromBody] string commentText)
    {
        // Call the service to update the comment
        var updatedUser = await _userService.UpdateComment(vendorId, commentId, commentText);

        // Return the updated comment data or an error if not found
        if (updatedUser != null)
        {
            return Ok(updatedUser); // Return the updated user object
        }

        return NotFound("Vendor or comment not found.");
    }

}
