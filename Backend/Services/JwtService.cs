using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

public class RefreshToken
{
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
}

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private static readonly Dictionary<string, RefreshToken> _refreshTokens = new(); // In-memory store for refresh tokens
    private static readonly Dictionary<string, string> _jwtTokens = new(); // In-memory store for JWT tokens
    private const int JwtExpirationHours = 1; // JWT token expiration time
    private const int RefreshTokenExpirationDays = 30; // Refresh token expiration time

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateJwt(User user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));

        // Customize claims based on your application needs
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id ?? throw new ArgumentNullException(nameof(user.Id))),
            new Claim(ClaimTypes.Role, user.Role ?? throw new ArgumentNullException(nameof(user.Role)))
        };

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
        }

        var keyString = _configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key is not configured.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer is not configured."),
            _configuration["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience is not configured."),
            claims,
            expires: DateTime.Now.AddHours(JwtExpirationHours),
            signingCredentials: creds);

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

        // Store the generated JWT token
        _jwtTokens[user.Id] = jwtToken;

        return jwtToken;
    }

    public string GenerateRefreshToken()
    {
        var refreshToken = Guid.NewGuid().ToString();
        return refreshToken;
    }

    public void StoreRefreshToken(string userId, string refreshToken)
    {
        // Store refresh token by user ID
        _refreshTokens[userId] = new RefreshToken
        {
            Token = refreshToken,
            Expiration = DateTime.UtcNow.AddDays(RefreshTokenExpirationDays) // Optionally set an expiration
        }; 
    }

    public void Logout(string jwtToken, string refreshToken)
    {
        // Find the user ID associated with the provided JWT token
        var userId = GetUserIdFromJwt(jwtToken);
        
        if (userId != null)
        {
            // Invalidate the refresh token
            if (_refreshTokens.ContainsKey(userId) && _refreshTokens[userId].Token == refreshToken)
            {
                _refreshTokens.Remove(userId); // Remove refresh token on logout
            }

            // Invalidate the JWT token
            if (_jwtTokens.ContainsKey(userId))
            {
                _jwtTokens.Remove(userId); // Remove JWT token on logout
            }

            // Additional logic if needed (like logging out the user, etc.)
            // Here you can add logic to update user status, log logout events, etc.
        }
    }

    public string GetUserIdFromJwt(string jwtToken) // Change this method to public
    {
        var handler = new JwtSecurityTokenHandler();
        if (handler.CanReadToken(jwtToken))
        {
            var token = handler.ReadJwtToken(jwtToken);
            return token.Subject; // Return the user ID from the JWT token
        }
        return null; // Invalid token
    }

    public bool IsJwtTokenValid(string userId, string token)
    {
        return _jwtTokens.TryGetValue(userId, out var storedToken) && storedToken == token;
    }

    public bool IsRefreshTokenValid(string userId, string refreshToken)
    {
        return _refreshTokens.TryGetValue(userId, out var storedToken) &&
               storedToken.Token == refreshToken &&
               storedToken.Expiration > DateTime.UtcNow; // Ensure refresh token hasn't expired
    }
}
