public interface IUserService
{
    // Authenticate a user by email and password
    Task<(User user, string jwtToken, string refreshToken)> Authenticate(string email, string password, string role);
    // Register a new user with email, username, password, and role
    Task<User> Register(string email, string username, string password, string role , string Address, int MobileNumber);

    // Check if the user has an Administrator role
    Task<bool> IsAdministrator(string userId);

    // Method to approve a customer account by the CSR
    Task<bool> ApproveCustomer(string customerId , bool isApproved);

    // Get a user by ID
    Task<User> GetUserById(string userId);

    // Get all users
    Task<IEnumerable<User>> GetAllUsers();

    // Update user information
    Task<bool> UpdateUser(string userId, User updatedUser);

    // Delete a user by ID
    Task<bool> DeleteUser(string userId);

    Task<User> GetUserByEmail(string email);

    Task<User> AddCommentAndUpdateRating(string vendorId, Comment newComment);
    
    Task<User> UpdateComment(string vendorId, string commentId, string commentText);

}
