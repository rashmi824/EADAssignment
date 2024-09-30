public interface IJwtService
{
    string GenerateJwt(User user);
    string GenerateRefreshToken();
    void StoreRefreshToken(string userId, string refreshToken);
    bool ValidateRefreshToken(string userId, string refreshToken);
    void InvalidateRefreshToken(string userId);
}
