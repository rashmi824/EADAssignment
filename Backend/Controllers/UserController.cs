using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;

    public UsersController(IUserService userService, IJwtService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }

    // Registration endpoint
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = await _userService.Register(dto.Email, dto.Username, dto.Password, dto.Role);
        if (user == null) 
            return BadRequest("User already exists");

        if (dto.Role == "Customer")
        {
            return Ok("Account created successfully. Your account is pending approval from CSR.");
        }

        return Ok("Account created successfully.");
    }

    // Login endpoint
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _userService.Authenticate(dto.Email, dto.Password);
        if (user == null) return Unauthorized("Invalid credentials");

        // Check if the user is a customer and whether their account is approved
        if (user.Role == "Customer" && user.IsApproved == false)
        {
            return Unauthorized("Your account is pending approval from CSR.");
        }

        var token = _jwtService.GenerateJwt(user);
        return Ok(new { token });
    }

    // Logout endpoint (if applicable, depending on your JWT implementation)
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // Implement your logout logic, e.g., by invalidating the token or removing it from client storage.
        return Ok("Logged out successfully.");
    }

    // Endpoint for CSR to approve customers
    [HttpPost("approve-customer/{customerId}")]
    public async Task<IActionResult> ApproveCustomer(string customerId)
    {
        var result = await _userService.ApproveCustomer(customerId);
        if (!result)
            return NotFound("Customer not found or is already approved.");

        return Ok("Customer approved successfully.");
    }

    // Get a user by ID
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(string userId)
    {
        var user = await _userService.GetUserById(userId);
        if (user == null) return NotFound("User not found");

        return Ok(user);
    }

    // Get all users
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsers();
        return Ok(users);
    }

    // Update user information
    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(string userId, [FromBody] User updatedUser)
    {
        var result = await _userService.UpdateUser(userId, updatedUser);
        if (!result) return NotFound("User not found");

        return Ok("User updated successfully.");
    }

    // Delete a user
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var result = await _userService.DeleteUser(userId);
        if (!result) return NotFound("User not found");

        return Ok("User deleted successfully.");
    }
}
