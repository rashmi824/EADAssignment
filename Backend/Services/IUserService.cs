public interface IUserService
{
    // Authenticate a user by email and password
    Task<User> Authenticate(string email, string password);

    // Register a new user with email, username, password, and role
    Task<User> Register(string email, string username, string password, string role);

    // Check if the user has an Administrator role
    Task<bool> IsAdministrator(string userId);

    // Method to approve a customer account by the CSR
    Task<bool> ApproveCustomer(string customerId);

    // Get a user by ID
    Task<User> GetUserById(string userId);

    // Get all users
    Task<IEnumerable<User>> GetAllUsers();

    // Update user information
    Task<bool> UpdateUser(string userId, User updatedUser);

    // Delete a user by ID
    Task<bool> DeleteUser(string userId);
}
