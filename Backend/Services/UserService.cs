using Backend.Data;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

public class UserService : IUserService
{
    private readonly IMongoCollection<User> _users;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IMongoDbContext dbContext, IPasswordHasher passwordHasher)
    {
        _users = dbContext.Users;
        _passwordHasher = passwordHasher;
    }

    // Authenticate method using email and password
    public async Task<User> Authenticate(string email, string password)
    {
        var user = await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        if (user == null || !_passwordHasher.Verify(password, user.PasswordHash))
            return null;

        return user;
    }

    // Register method
    public async Task<User> Register(string email, string username, string password, string role)
    {
        var existingUser = await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        if (existingUser != null) return null; // User already exists

        var user = new User
        {
            Email = email,
            Username = username,
            PasswordHash = _passwordHasher.Hash(password),
            Role = role,
            IsApproved = role == "Customer" ? false : (bool?)null // Only unapproved for "Customer" role
        };

        await _users.InsertOneAsync(user);

        // If user is a customer, send notification to CSR for approval
        if (user.Role == "Customer")
        {
            await NotifyCsrForApproval(user); // Notify CSR about pending customer approval
        }

        return user;
    }

    // ApproveCustomer method
    public async Task<bool> ApproveCustomer(string customerId)
    {
        var update = Builders<User>.Update.Set(u => u.IsApproved, true);
        var result = await _users.UpdateOneAsync(u => u.Id == customerId && u.Role == "Customer", update);
        return result.ModifiedCount > 0;
    }

    // Notify CSR method
    private async Task NotifyCsrForApproval(User user)
    {
        Console.WriteLine($"New customer registration pending approval: {user.Id}");
    }

    // Check if the user is an administrator
    public async Task<bool> IsAdministrator(string userId)
    {
        var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
        return user?.Role == "Administrator";
    }

    // Get a user by ID
    public async Task<User> GetUserById(string userId)
    {
        return await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
    }

    // Get all users
    public async Task<IEnumerable<User>> GetAllUsers()
    {
        return await _users.Find(_ => true).ToListAsync(); // Return as IEnumerable<User>
    }

    // Update user information
    public async Task<bool> UpdateUser(string userId, User updatedUser)
    {
        var result = await _users.ReplaceOneAsync(u => u.Id == userId, updatedUser);
        return result.ModifiedCount > 0;
    }

    // Delete a user
    public async Task<bool> DeleteUser(string userId)
    {
        var result = await _users.DeleteOneAsync(u => u.Id == userId);
        return result.DeletedCount > 0;
    }
}
