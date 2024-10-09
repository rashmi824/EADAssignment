/*
 UsersController.cs
 This file contains endpoints for user registration, login, approval, user management, and authentication.
 */

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;

    // Constructor to initialize user and JWT services
    public UsersController(IUserService userService, IJwtService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }

    // Registration endpoint for new users
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        // Register the user and check if they already exist
        var user = await _userService.Register(dto.Email, dto.Username, dto.Password, dto.Role, dto.Address, dto.MobileNumber);
        if (user == null) 
            return BadRequest("User already exists");

        // Notify CSR for approval if the user is a customer
        if (dto.Role == "Customer")
        {
            _userService.NotifyCsrForApproval(user);
            return Ok("Account created successfully. Your account is pending approval from CSR.");
        }

        return Ok("Account created successfully.");
    }

    // Login endpoint for users
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        // Authenticate user and retrieve JWT and refresh tokens
        var (user, jwtToken, refreshToken) = await _userService.Authenticate(dto.Email, dto.Password, dto.Role);
        
        if (user == null)
            return Unauthorized("Invalid credentials");

        // Check for CSR approval and account status for customers
        if (user.Role == "Customer")
        {
            if (!(user.IsApproved ?? false))
            {
                return Unauthorized("Your account is pending approval from CSR.");
            }
            
            if (!(user.Status ?? false))
            {
                return Unauthorized("Your account has been deactivated.");
            }
        }

        // Return tokens if login is successful
        return Ok(new { token = jwtToken, refreshToken });
    }

    // Endpoint for CSR to approve/disapprove customers
    [HttpPut("approve-customer/{customerId}")]
    public async Task<IActionResult> ApproveCustomer(string customerId, [FromBody] bool isApproved)
    {
        // Approve or disapprove customer based on input
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _userService.ApproveCustomer(customerId, isApproved);
        if (!result)
            return NotFound("Customer not found or already " + (isApproved ? "approved." : "disapproved."));

        return Ok(isApproved ? "Customer approved successfully." : "Customer disapproved successfully.");
    }

    // Get a specific user by ID
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(string userId)
    {
        // Retrieve user by ID
        var user = await _userService.GetUserById(userId);
        if (user == null) return NotFound("User not found");

        return Ok(user);
    }

    // Retrieve user ID from the provided JWT token
    [HttpGet("user-id")]
    public IActionResult GetUserIdFromToken([FromHeader(Name = "Authorization")] string jwtToken)
    {
        if (jwtToken.StartsWith("Bearer "))
        {
            jwtToken = jwtToken.Substring("Bearer ".Length).Trim();
        }

        // Use JWT service to extract user ID from token
        var userId = _jwtService.GetUserIdFromJwt(jwtToken);

        if (userId == null)
        {
            return Unauthorized("Invalid token.");
        }

        return Ok(new { UserId = userId });
    }

    // Get a list of all users
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        // Retrieve all users from the system
        var users = await _userService.GetAllUsers();
        return Ok(users);
    }

    // Update user information
    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(string userId, [FromBody] RegisterDto userUpdateDto)
    {
        // Attempt to update user details
        var result = await _userService.UpdateUser(
            userId,
            email: userUpdateDto.Email,
            username: userUpdateDto.Username,
            password: userUpdateDto.Password,
            role: userUpdateDto.Role,
            address: userUpdateDto.Address,
            mobileNumber: userUpdateDto.MobileNumber
        );

        // Check if the user was found and updated successfully
        if (!result)
        {
            // Find the user by ID to determine the reason for failure
            var user = await _userService.GetUserById(userId); // Assuming GetUserById fetches the user by ID

            if (user == null)
            {
                return NotFound("User not found."); // User does not exist
            }

            // If the user exists but the email update failed due to a conflict, return a conflict status
            if (!string.IsNullOrEmpty(userUpdateDto.Email) && user.Email != userUpdateDto.Email)
            {
                // Here, we assume the UpdateUser method has already handled the conflict check
                return Conflict("Email already exists for another user."); // Email conflict
            }
        }

        return Ok("User updated successfully.");
    }




    // Delete a user by ID
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        // Delete user from the system
        var result = await _userService.DeleteUser(userId);
        if (!result) return NotFound("User not found");

        return Ok("User deleted successfully.");
    }

    // Logout user by invalidating their token(s)
    [HttpPost("logout")]
    public IActionResult Logout([FromBody] LogoutDto dto)
    {
        // Validate token or refresh token for logout
        if (string.IsNullOrEmpty(dto.Token) && string.IsNullOrEmpty(dto.RefreshToken))
            return BadRequest("Token or refresh token is required.");

        // Call the JwtService logout method
        _jwtService.Logout(dto.Token, dto.RefreshToken);
        return Ok("Logged out successfully.");
    }

    // Activate a user account by ID
    [HttpPut("activate/{id}")]
    public async Task<IActionResult> ActivateUser(string id)
    {
        // Activate user account
        var updatedUser = await _userService.UpdateUserStatus(id, true);
        if (updatedUser != null)
        {
            return Ok(new 
            { 
                Message = "User activated successfully.",
                User = updatedUser
            });
        }
        return NotFound(new { Message = "User not found or User already Activated" });
    }

    // Deactivate a user account by ID
    [HttpPut("deactivate/{id}")]
    public async Task<IActionResult> DeactivateUser(string id)
    {
        // Deactivate user account
        var updatedUser = await _userService.UpdateUserStatus(id, false);
        if (updatedUser != null)
        {
            return Ok(new 
            { 
                Message = "User deactivated successfully.",
                User = updatedUser
            });
        }
        return NotFound(new { Message = "User not found or User Already Deactivated" });
    }
}
