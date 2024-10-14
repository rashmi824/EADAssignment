// IUserService.cs
// This interface defines user management functionalities including authentication, registration, approval,
// updates, and retrieval of user information. It serves as a contract for the UserService implementation.

using System.Collections.Generic;
using System.Threading.Tasks;

public interface IUserService
{
    // Authenticate a user by email and password and return the user along with JWT and refresh tokens
    Task<(User user, string jwtToken, string refreshToken)> Authenticate(string email, string password, string role);
    
    // Register a new user with email, username, password, and role, along with address and mobile number
    Task<User> Register(string email, string username, string password, string role, string address, int mobileNumber);
    
    // Check if the user has an Administrator role
    Task<bool> IsAdministrator(string userId);
    
    // Approve a customer account by the CSR (Customer Service Representative)
    Task<bool> ApproveCustomer(string customerId, bool isApproved);
    
    // Get a user by their unique ID
    Task<User> GetUserById(string userId);
    
    // Retrieve all users from the database
    Task<IEnumerable<User>> GetAllUsers();
    
    // Update user information, allowing optional updates for various fields
    Task<bool> UpdateUser(string userId, string email = null, string username = null, string password = null, string role = null, string address = null, int? mobileNumber = null);
    
    // Delete a user by their unique ID
    Task<bool> DeleteUser(string userId);
    
    // Get a user by their email address
    Task<User> GetUserByEmail(string email);
    
    // Add a comment and update the vendor's rating based on new feedback
    Task<User> AddCommentAndUpdateRating(string vendorId, Comment newComment);
    
    // Update an existing comment based on its ID with new text
    Task<User> UpdateComment(string vendorId, string commentId, string commentText);
    
    // Notify CSR for approval of a new user registration
    Task NotifyCsrForApproval(User newUser);
    
    // Update the status of a user (active/inactive)
    Task<User> UpdateUserStatus(string userId, bool status);
    
}
