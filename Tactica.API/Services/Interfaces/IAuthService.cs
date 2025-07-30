using Tactica.API.DTOs.Auth;
using Tactica.API.Repositories.Interfaces;

namespace Tactica.API.Services.Interfaces;

/// <summary>
/// Defines authentication-related operations such as user registration
/// and login.
/// </summary>
public interface IAuthService
{

    /// <summary>
    /// Registers a new user and returns an authentication response with a JWT token.
    /// </summary>
    /// <param name="request">The registration request containing email and password.</param>
    /// <returns>The authentication response including the JWT token.</returns>
    Task<AuthResponse> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Authenticates a user and returns an authentication response with a JWT token.
    /// </summary>
    /// <param name="request">The login request containing credentials.</param>
    /// <returns>The authentication response including the JWT token.</returns>z
    Task<AuthResponse> LoginAsync(LoginRequest request);

    /// <summary>
    /// Validates a refresh token and returns a new access token
    /// and refresh token pair.
    /// </summary>
    /// <param name="refreshToken">The refresh token string to validate.</param>
    /// <returns>
    /// An <see cref="AuthService"/> containing the new access token and
    /// refresh token.
    /// </returns>
    /// <exception cref="UnauthorizedAccessException">
    /// Throws when the refresh token is invalid or expired.
    /// </exception> 
    Task<AuthResponse> RefreshTokenAsync(string refreshToken);
}