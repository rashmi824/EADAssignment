// LogoutDto.cs
// This class is a Data Transfer Object (DTO) used for user logout operations. 
// It contains the JWT token and optional refresh token to invalidate the user's session.

public class LogoutDto
{
    public string Token { get; set; } // JWT token to be invalidated during logout
    public string RefreshToken { get; set; } // Optional refresh token for additional session management
}
