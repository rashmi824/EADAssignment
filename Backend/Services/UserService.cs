// UserService.cs
// This class handles user-related operations such as authentication, registration, approval, and notification.

using Backend.Data;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson; // Ensure this is included for ObjectId generation
using System.Security.Claims; // Include this for Claims

public class UserService : IUserService
{
    private readonly IMongoCollection<User> _users; // MongoDB collection for users
    private readonly IPasswordHasher _passwordHasher; // Service for hashing passwords
    private readonly IJwtService _jwtService; // Service for JWT operations
    private readonly GmailService _gmailService; // Service for sending emails

    public UserService(IMongoDbContext dbContext, IPasswordHasher passwordHasher, IJwtService jwtService, GmailService gmailService)
    {
        _users = dbContext.Users; // Initialize users collection from database context
        _passwordHasher = passwordHasher; // Inject password hasher service
        _jwtService = jwtService; // Inject JWT service
        _gmailService = gmailService; // Inject Gmail service
    }

    // Authenticate method using email, password, and role
    public async Task<(User user, string jwtToken, string refreshToken)> Authenticate(string email, string password, string role)
    {
        // Find the user by email and check their role
        var user = await _users.Find(u => u.Email == email && (role == "Customer" ? u.Role == "Customer" : u.Role != "Customer")).FirstOrDefaultAsync();

        // Validate user existence and password correctness
        if (user == null || !_passwordHasher.Verify(password, user.PasswordHash))
            return (null, null, null); // Return null if user not found or password is incorrect

        // Generate JWT and refresh token
        var jwtToken = _jwtService.GenerateJwt(user);
        var refreshToken = _jwtService.GenerateRefreshToken();
        _jwtService.StoreRefreshToken(user.Id, refreshToken); // Store the refresh token for future use

        return (user, jwtToken, refreshToken); // Return the authenticated user and tokens
    }

    // Register method for creating a new user
    public async Task<User> Register(string email, string username, string password, string role, string address, int mobileNumber)
    {
        // Ensure unique email for customers
        if (role == "Customer")
        {
            var existingCustomer = await _users.Find(u => u.Email == email && u.Role == "Customer").FirstOrDefaultAsync();
            if (existingCustomer != null) return null; // Customer already exists with the same email
        }

        // Ensure unique email for non-customer roles
        if (role != "Customer")
        {
            var existingUser = await _users.Find(u => u.Email == email && u.Role != "Customer").FirstOrDefaultAsync();
            if (existingUser != null) return null; // Email is already in use by another user
        }

        // Create a new user object
        var user = new User
        {
            Email = email,
            Username = username,
            PasswordHash = _passwordHasher.Hash(password), // Hash the password before storing
            Role = role,
            IsApproved = role == "Customer" ? false : true, // Unapproved status for "Customer"
            Status = true, // Set user status to active
            Address = address,
            MobileNumber = mobileNumber,
            VendorDetails = role == "Vendor" ? new Vendor() : null // Initialize Vendor details if role is Vendor
        };

        // Insert the new user into the database
        await _users.InsertOneAsync(user);
        NotifyCsrForApproval(user);

        return user; // Return the registered user
    }

    // Notify CSR for approval of new customer registration
    public async Task NotifyCsrForApproval(User newUser)
    {
        var csrUsers = await _users.Find(u => u.Role == "CSR" || u.Role == "Administrator").ToListAsync(); // Retrieve all CSR users
        string subject = "New Customer Registration Pending Approval";
        string message = $"A new customer has registered. Username: {newUser.Username}, Email: {newUser.Email}. Please review and approve.";

        int successfulEmails = 0; // Counter for successfully sent emails
        int failedEmails = 0; // Counter for failed emails

        foreach (var csr in csrUsers)
        {
            try
            {
                // Use the GmailService to send the notification email
                await _gmailService.SendEmailAsync(csr.Email, subject, message);
                Console.WriteLine($"Email successfully sent to {csr.Email}."); // Log success
                successfulEmails++; // Increment success counter
            }
            catch (Exception ex)
            {
                // Log the error (consider implementing a logger)
                Console.WriteLine($"Error sending email to {csr.Email}: {ex.Message}");
                failedEmails++; // Increment failure counter
            }
        }

        // Log the result of email notifications
        Console.WriteLine($"Emails sent successfully: {successfulEmails}, Emails failed: {failedEmails}");
    }

    // ApproveCustomer method to change approval status of a customer
    public async Task<bool> ApproveCustomer(string customerId, bool isApproved)
    {
        var update = Builders<User>.Update.Set(u => u.IsApproved, isApproved); // Create update definition
        var result = await _users.UpdateOneAsync(u => u.Id == customerId && u.Role == "Customer", update); // Update customer approval status
        return result.ModifiedCount > 0; // Return true if the update was successful
    }

    // Check if the user is an administrator
    public async Task<bool> IsAdministrator(string userId)
    {
        var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync(); // Find user by ID
        return user?.Role == "Administrator"; // Return true if user is an administrator
    }

    // Get a user by their ID
    public async Task<User> GetUserById(string userId)
    {
        return await _users.Find(u => u.Id == userId).FirstOrDefaultAsync(); // Return user by ID
    }

    // Get all users
    public async Task<IEnumerable<User>> GetAllUsers()
    {
        return await _users.Find(_ => true).ToListAsync(); // Return all users
    }

   // Update user information
    public async Task<bool> UpdateUser(string userId, string email = null, string username = null, string password = null, string role = null, string address = null, int? mobileNumber = null)
    {
        // Find the user by ID to retrieve their existing details
        var existingUser = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();

        // If the user does not exist, return false
        if (existingUser == null) return false; // User not found

        // Check if the email is being updated and already exists for another user
        if (!string.IsNullOrEmpty(email) && existingUser.Email != email) // Only check if the email is different
        {
            // Check for email conflict based on user role
            if (role == "Customer")
            {
                var customerWithSameEmail = await _users.Find(u => u.Email == email && u.Role == "Customer" && u.Id != userId).FirstOrDefaultAsync();
                if (customerWithSameEmail != null) return false; // Email already exists for another customer
            }
            else // For non-customer roles
            {
                var nonCustomerWithSameEmail = await _users.Find(u => u.Email == email && u.Role != "Customer" && u.Id != userId).FirstOrDefaultAsync();
                if (nonCustomerWithSameEmail != null) return false; // Email already exists for another non-customer user
            }
        }

        // Update the user details if provided (not null or empty)
        if (!string.IsNullOrEmpty(email)) existingUser.Email = email; // Update email
        if (!string.IsNullOrEmpty(username)) existingUser.Username = username; // Update username
        if (!string.IsNullOrEmpty(address)) existingUser.Address = address; // Update address
        if (mobileNumber.HasValue) existingUser.MobileNumber = mobileNumber.Value; // Update mobile number

        // Hash and update the password if provided
        if (!string.IsNullOrEmpty(password))
        {
            existingUser.PasswordHash = _passwordHasher.Hash(password);
        }

        // Update the user in the database
        var result = await _users.ReplaceOneAsync(u => u.Id == userId, existingUser);
        return result.ModifiedCount > 0; // Return true if the update was successful
    }

    // Delete a user
    public async Task<bool> DeleteUser(string userId)
    {
        var result = await _users.DeleteOneAsync(u => u.Id == userId); // Delete user by ID
        return result.DeletedCount > 0; // Return true if the user was deleted
    }

    // Get a user by email
    public async Task<User> GetUserByEmail(string email)
    {
        return await _users.Find(u => u.Email == email).FirstOrDefaultAsync(); // Return user by email
    }

    // Add a comment and update vendor rating
    public async Task<User> AddCommentAndUpdateRating(string vendorId, Comment newComment)
    {
        newComment.Id = ObjectId.GenerateNewId().ToString(); // Generate a new unique ID for the comment
        var vendor = await _users.Find(u => u.Id == vendorId && u.Role == "Vendor").FirstOrDefaultAsync(); // Find the vendor

        if (vendor == null || vendor.VendorDetails == null) return null; // Vendor not found

        // Add the new comment to the vendor's comment list
        vendor.VendorDetails.Comments.Add(newComment);

        // Recalculate the average rating
        var totalRating = vendor.VendorDetails.Comments.Sum(c => c.Rating); // Sum of all ratings
        vendor.VendorDetails.AverageRating = (double)totalRating / vendor.VendorDetails.Comments.Count; // Calculate average rating

        // Update the vendor in the database
        var result = await _users.ReplaceOneAsync(u => u.Id == vendorId, vendor);
        return result.ModifiedCount > 0 ? vendor : null; // Return the updated vendor or null if the update failed
    }

    // Update a specific comment
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

    // Update user status (active/inactive)
    public async Task<User> UpdateUserStatus(string userId, bool status)
    {
        var update = Builders<User>.Update.Set(u => u.Status, status); // Create update definition

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
