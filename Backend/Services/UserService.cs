using Backend.Data;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson; // Make sure this is included
using System.Security.Claims; // Include this for Claims

public class UserService : IUserService
{
    private readonly IMongoCollection<User> _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly GmailService _gmailService;




    public UserService(IMongoDbContext dbContext, IPasswordHasher passwordHasher, IJwtService jwtService,GmailService gmailService)
    {
        _users = dbContext.Users;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService; // Inject JWT service
        _gmailService = gmailService;
    }

    // Authenticate method using email and password and Role
    public async Task<(User user, string jwtToken, string refreshToken)> Authenticate(string email, string password, string role)
    {
        // Find the user by email
        var user = await _users.Find(u => u.Email == email && (role == "Customer" ? u.Role == "Customer" : u.Role != "Customer")).FirstOrDefaultAsync();

        // Check if the user exists and if the password is correct
        if (user == null || !_passwordHasher.Verify(password, user.PasswordHash))
            return (null, null, null); // Return null if user not found or password is incorrect

        // Generate JWT and refresh token
        var jwtToken = _jwtService.GenerateJwt(user);
        var refreshToken = _jwtService.GenerateRefreshToken();
        _jwtService.StoreRefreshToken(user.Id, refreshToken); // Store the refresh token

        return (user, jwtToken, refreshToken); // Return the authenticated user and tokens
    }


    // Register method
    public async Task<User> Register(string email, string username, string password, string role, string address, int mobileNumber)
    {
        // Check if the role is "Customer" and ensure the email is unique among customers
        if (role == "Customer")
        {
            var existingCustomer = await _users.Find(u => u.Email == email && u.Role == "Customer").FirstOrDefaultAsync();
            if (existingCustomer != null) return null; // Customer already exists with the same email
        }

        if (role != "Customer")
        {
            // Check if the email is already used by another user who is not a customer
            var existingUser = await _users.Find(u => u.Email == email && u.Role != "Customer").FirstOrDefaultAsync();
            if (existingUser != null) return null; // Email is already in use by another user
        }

        var user = new User
        {
            Email = email,
            Username = username,
            PasswordHash = _passwordHasher.Hash(password),
            Role = role,
            IsApproved = role == "Customer" ? false : true, // Only unapproved for "Customer" role
            Status = true,
            Address = address,
            MobileNumber = mobileNumber,
            VendorDetails = role == "Vendor" ? new Vendor() : null // Set Vendor details if role is Vendor
        };

        await _users.InsertOneAsync(user);

       

        return user;
    }

    public async Task NotifyCsrForApproval(User newUser)
{
    var csrUsers = await _users.Find(u => u.Role == "CSR").ToListAsync();
    string subject = "New Customer Registration Pending Approval";
    string message = $"A new customer has registered. Username: {newUser.Username}, Email: {newUser.Email}. Please review and approve.";

    int successfulEmails = 0;
    int failedEmails = 0;

    foreach (var csr in csrUsers)
    {
        try
        {
            // Use the centralized GmailService to send the notification
            await _gmailService.SendEmailAsync(csr.Email, subject, message);
            Console.WriteLine($"Email successfully sent to {csr.Email}.");
            successfulEmails++; // Count successful email
        }
        catch (Exception ex)
        {
            // Log the error (you can implement a logger or handle the exception)
            Console.WriteLine($"Error sending email to {csr.Email}: {ex.Message}");
            failedEmails++; // Count failed email
        }
    }

    // Log the result of email notifications
    Console.WriteLine($"Emails sent successfully: {successfulEmails}, Emails failed: {failedEmails}");

    
}


    // ApproveCustomer method
    public async Task<bool> ApproveCustomer(string customerId, bool isApproved)
    {
        var update = Builders<User>.Update.Set(u => u.IsApproved, isApproved);
        var result = await _users.UpdateOneAsync(u => u.Id == customerId && u.Role == "Customer", update);
        return result.ModifiedCount > 0; // Return true if the update was successful
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

    // New method to get a user by email
    public async Task<User> GetUserByEmail(string email)
    {
        return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

    public async Task<User> AddCommentAndUpdateRating(string vendorId, Comment newComment)
    {
        // Set a new unique ID for the comment
        newComment.Id = ObjectId.GenerateNewId().ToString(); // Generate a new ObjectId
        // Find the vendor by ID
        var vendor = await _users.Find(u => u.Id == vendorId && u.Role == "Vendor").FirstOrDefaultAsync();

        if (vendor == null || vendor.VendorDetails == null) return null; // Vendor not found

        // Add the new comment to the vendor's comment list
        vendor.VendorDetails.Comments.Add(newComment);

        // Recalculate the average rating
        var totalRating = vendor.VendorDetails.Comments.Sum(c => c.Rating);
        var commentCount = vendor.VendorDetails.Comments.Count;
        vendor.VendorDetails.AverageRating = totalRating / commentCount;

        // Update the vendor in the database
        var updateResult = await _users.ReplaceOneAsync(u => u.Id == vendorId, vendor);

        if (updateResult.ModifiedCount > 0)
        {
            return vendor; // Return the updated vendor
        }

        return null; // Return null if the update failed
    }

    public async Task<User> UpdateComment(string vendorId, string commentId, string commentText)
    {
        // Find the vendor by ID
        var vendor = await _users.Find(u => u.Id == vendorId && u.Role == "Vendor").FirstOrDefaultAsync();

        if (vendor == null || vendor.VendorDetails == null) return null; // Vendor not found

        // Find the specific comment by ID
        var comment = vendor.VendorDetails.Comments.FirstOrDefault(c => c.Id == commentId);
        if (comment == null) return null; // Comment not found

        // Update the comment text
        comment.CommentText = commentText;

        // Recalculate the average rating after the update
        var totalRating = vendor.VendorDetails.Comments.Sum(c => c.Rating);
        var commentCount = vendor.VendorDetails.Comments.Count;
        vendor.VendorDetails.AverageRating = totalRating / commentCount;

        // Update the vendor in the database
        var updateResult = await _users.ReplaceOneAsync(u => u.Id == vendorId, vendor);

        if (updateResult.ModifiedCount > 0)
        {
            return vendor; // Return the updated vendor
        }

        return null; // Return null if the update failed
    }

 public async Task<User> UpdateUserStatus(string userId, bool status)
{
      
    var update = Builders<User>.Update.Set(u => u.Status, status);

    // Update the user status
     var result = await _users.UpdateOneAsync(u => u.Id == userId, update);
    
    // Check if the user was modified
    if (result.ModifiedCount > 0)
    {
        // If modified, retrieve the updated user
        var updatedUser = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
        return updatedUser; // Return the updated user object
    }

    return null; // Return null if the user was not found or not modified
}

}
