// IPasswordHasher.cs
// This interface defines methods for hashing passwords and verifying hashed passwords.
public interface IPasswordHasher
{
    // Hashes the provided password and returns the hashed value.
    string Hash(string password);

    // Verifies that the provided password matches the stored hash.
    bool Verify(string password, string hash);
}
