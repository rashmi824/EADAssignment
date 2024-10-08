// JwtService.cs
// This class implements IJwtService to manage JWT and refresh token generation, validation, and storage.
// It utilizes in-memory dictionaries to store the tokens and expiration information.

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
        _configuration = configuration; // Initialize configuration
    }

    // Generates a JWT token for the specified user.
    public string GenerateJwt(User user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user)); // Ensure user is not null

        // Customize claims based on your application needs
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id ?? throw new ArgumentNullException(nameof(user.Id))), // User ID as subject
            new Claim(ClaimTypes.Role, user.Role ?? throw new ArgumentNullException(nameof(user.Role))) // User role claim
        };

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            claims.Add(new Claim(ClaimTypes.Email, user.Email)); // Email claim if available
        }

        // Retrieve the key from configuration and create signing credentials
        var keyString = _configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key is not configured.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Create the JWT token
        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer is not configured."),
            _configuration["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience is not configured."),
            claims,
            expires: DateTime.Now.AddHours(JwtExpirationHours), // Set expiration time
            signingCredentials: creds);

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token); // Write the token to a string

        // Store the generated JWT token
        _jwtTokens[user.Id] = jwtToken;

        return jwtToken; // Return the generated JWT token
    }

    // Generates a new refresh token.
    public string GenerateRefreshToken()
    {
        var refreshToken = Guid.NewGuid().ToString(); // Create a new GUID as refresh token
        return refreshToken; // Return the refresh token
    }

    // Stores the refresh token associated with the user ID.
    public void StoreRefreshToken(string userId, string refreshToken)
    {
        // Store refresh token by user ID
        _refreshTokens[userId] = new RefreshToken
        {
            Token = refreshToken,
            Expiration = DateTime.UtcNow.AddDays(RefreshTokenExpirationDays) // Set an expiration time for the refresh token
        };
    }

    // Logs out the user by invalidating the JWT and refresh tokens.
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

    // Retrieves the user ID from the provided JWT token.
    public string GetUserIdFromJwt(string jwtToken) // Change this method to public
    {
        var handler = new JwtSecurityTokenHandler();
        if (handler.CanReadToken(jwtToken))
        {
            var token = handler.ReadJwtToken(jwtToken);
            return token.Subject; // Return the user ID from the JWT token
        }
        return null; // Return null if the token is invalid
    }

    // Checks if the provided JWT token is valid for the given user ID.
    public bool IsJwtTokenValid(string userId, string token)
    {
        return _jwtTokens.TryGetValue(userId, out var storedToken) && storedToken == token; // Validate the token
    }

    // Checks if the provided refresh token is valid for the given user ID.
    public bool IsRefreshTokenValid(string userId, string refreshToken)
    {
        return _refreshTokens.TryGetValue(userId, out var storedToken) &&
               storedToken.Token == refreshToken &&
               storedToken.Expiration > DateTime.UtcNow; // Ensure refresh token hasn't expired
    }
}
