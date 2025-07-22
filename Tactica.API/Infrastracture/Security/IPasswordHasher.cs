namespace Tactica.API.Infrastructure.Security;

/// <summary>
/// Defines contract for hashing and verifying passwords.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes the provided plain-text password.
    /// </summary>
    /// <param name="password">The plain-text password to hash.</param>
    /// <returns>Hashed password as a base64 string.</returns>
    string Hash(string password);
}