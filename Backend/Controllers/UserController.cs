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
        var user = await _userService.Register(dto.Email, dto.Username, dto.Password, dto.Role, dto.Address, dto.MobileNumber);
        if (user == null) 
            return BadRequest("User already exists");

        if (dto.Role == "Customer")
        {
            _userService.NotifyCsrForApproval(user);
            return Ok("Account created successfully. Your account is pending approval from CSR.");
        }

        return Ok("Account created successfully.");
    }

    // Login endpoint
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var (user, jwtToken, refreshToken) = await _userService.Authenticate(dto.Email, dto.Password, dto.Role);
        if (user == null) return Unauthorized("Invalid credentials");

        // Check if the user is a customer and whether their account is approved
        // Check if the user is a customer and whether their account is approved
    if (user.Role == "Customer" && (user.IsApproved ?? false) == false)
    {
        return Unauthorized("Your account is pending approval from CSR.");
    }


        return Ok(new { token = jwtToken, refreshToken }); // Return both tokens
    }

    

    // Endpoint for CSR to approve or disapprove customers
    [HttpPut("approve-customer/{customerId}")]
    public async Task<IActionResult> ApproveCustomer(string customerId, [FromBody] bool isApproved)
    {
        // Validate the input
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _userService.ApproveCustomer(customerId, isApproved);
        if (!result)
            return NotFound("Customer not found or update failed.");

        return Ok(isApproved ? "Customer approved successfully." : "Customer disapproved successfully.");
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

    // Logout endpoint
    [HttpPost("logout")]
    public IActionResult Logout([FromBody] LogoutDto dto) // Assuming LogoutDto contains token and optional refreshToken
    {
        if (string.IsNullOrEmpty(dto.Token) && string.IsNullOrEmpty(dto.RefreshToken))
            return BadRequest("Token or refresh token is required.");

        // Call the JwtService logout method
        _jwtService.Logout(dto.Token, dto.RefreshToken); // Pass both tokens
        return Ok("Logged out successfully.");
    }



}
