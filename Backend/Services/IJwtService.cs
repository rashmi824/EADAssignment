public interface IJwtService
{
    // Generates a JWT for the specified user
    string GenerateJwt(User user);
    string GenerateRefreshToken(); // Should return a string
    void StoreRefreshToken(string userId, string refreshToken); // Accept userId and refreshToken as string
    void Logout(string jwtToken, string refreshToken);
    bool IsJwtTokenValid(string userId, string token);
    bool IsRefreshTokenValid(string userId, string refreshToken);
    string GetUserIdFromJwt(string jwtToken);
}



