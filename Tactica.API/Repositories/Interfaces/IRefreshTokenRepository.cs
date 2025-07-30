using Tactica.API.Models;

namespace Tactica.API.Repositories.Interfaces;

/// <summary>
/// Defines operations for storing and managing refresh tokens.
/// </summary>
public interface IRefreshTokenRepository
{
    /// <summary>
    /// Saves a new refresh token to the database.
    /// </summary>
    /// <param name="token">The refresh token to save.</param>
    Task SaveAsync(RefreshToken token);

    /// <summary>
    /// Retrieves a refresh token and its associated user by token string,
    /// only if it hasn't been revoked.
    /// </summary>
    /// <param name="token">The refresh token string.</param>
    /// <returns>The refresh token or null if not found or revoked.</returns>
    Task<RefreshToken?> GetByTokenAsync(string token);

    /// <summary>
    /// Marks the given refresh token as revoked in the database.
    /// </summary>
    /// <param name="token">The refresh token to revoke.</param>
    Task RevokeAsync(RefreshToken token);
}