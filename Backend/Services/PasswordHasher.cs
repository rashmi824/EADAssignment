// PasswordHasher.cs
// This class implements the IPasswordHasher interface to provide methods for hashing and verifying passwords.
// It uses BCrypt.Net for secure password hashing.

public class PasswordHasher : IPasswordHasher
{
    // Hashes the given password using BCrypt and returns the hashed password.
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password); // Hash the password securely
    }

    // Verifies a given password against a previously hashed password.
    public bool Verify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash); // Check if the password matches the hash
    }
}
