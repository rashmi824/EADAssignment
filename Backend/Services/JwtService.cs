using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private static readonly Dictionary<string, string> _refreshTokens = new(); // In-memory store for refresh tokens

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateJwt(User user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id ?? throw new ArgumentNullException(nameof(user.Id))),
            new Claim(ClaimTypes.Role, user.Role ?? throw new ArgumentNullException(nameof(user.Role)))
        };

        var keyString = _configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key is not configured.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer is not configured."),
            _configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer is not configured."),
            claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var refreshToken = Guid.NewGuid().ToString();
        return refreshToken;
    }

    public void StoreRefreshToken(string userId, string refreshToken)
    {
        _refreshTokens[userId] = refreshToken; // Store refresh token by user ID
    }

    public bool ValidateRefreshToken(string userId, string refreshToken)
    {
        return _refreshTokens.TryGetValue(userId, out var storedRefreshToken) && storedRefreshToken == refreshToken;
    }

    public void InvalidateRefreshToken(string userId)
    {
        _refreshTokens.Remove(userId); // Remove refresh token on logout
    }
}
