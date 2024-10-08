// LoginDto.cs
// This class is a Data Transfer Object (DTO) used for user login operations. 
// It contains the necessary information for authenticating a user, 
// including their email, password, and role.

public class LoginDto
{
    public required string Email { get; set; } // User's email for authentication
    public required string Password { get; set; } // User's password for authentication

    public required string Role { get; set; } // Role of the user (e.g., Customer, Administrator, etc.)
}
