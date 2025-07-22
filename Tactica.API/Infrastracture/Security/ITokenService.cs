using Tactica.API.Models;

namespace Tactica.API.Infrastructure.Security;

/// <summary>
/// Defines contract for generating JWT tokens for authenticated users.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a signed JWT access token for the provided user.
    /// </summary>
    /// <param name="user">The user for whom the token is generated.</param>
    /// <returns>A signed JWT token string.</returns>
    string GenerateToken(User user);
}