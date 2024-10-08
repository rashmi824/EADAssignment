// RegisterDto.cs
// This class is a Data Transfer Object (DTO) used for user registration operations.
// It contains the necessary information required to register a new user.

public class RegisterDto
{
    public required string Email { get; set; } // User's email, used as a unique identifier
    public required string Username { get; set; } // User's chosen username
    public required string Password { get; set; } // User's password, to be hashed before storage
    public required string Role { get; set; } // Role of the user (e.g., Administrator, Vendor, Customer, CSR)
    public required string Address { get; set; } // User's address for delivery or account purposes
    public required int MobileNumber { get; set; } // User's mobile number for contact purposes
}
