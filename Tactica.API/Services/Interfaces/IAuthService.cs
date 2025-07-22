using Tactica.API.DTOs.Auth;

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
}