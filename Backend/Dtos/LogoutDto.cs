public class LogoutDto
{
    public string Token { get; set; } // JWT token
    public string RefreshToken { get; set; } // Optional refresh token
}
