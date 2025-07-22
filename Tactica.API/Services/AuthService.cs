using Tactica.API.DTOs.Auth;
using Tactica.API.Infrastructure.Security;
using Tactica.API.Models;
using Tactica.API.Repositories.Interfaces;
using Tactica.API.Services.Interfaces;

namespace Tactica.API.Services;

/// <summary>
/// Provides implementation for user authentication logic
/// including registration and login.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(
        IUserRepository userRepo,
        IPasswordHasher passwordHasher,
        ITokenService tokenService
    )
    {
        _userRepo = userRepo;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    /// <inheritdoc /> 
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepo.GetByEmailAsync(request.Email);
        if (existingUser != null)
            throw new InvalidOperationException("Email already in use.");

        var hashedPassword = _passwordHasher.Hash(request.Password);

        var newUser = new User
        {
            Email = request.Email,
            PasswordHash = hashedPassword
        };

        await _userRepo.AddUserAsync(newUser);

        var token = _tokenService.GenerateToken(newUser);

        return new AuthResponse
        {
            Email = newUser.Email,
            Token = token
        };
    }

    /// <inheritdoc /> 
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepo.GetByEmailAsync(request.Email);
        if (user == null || _passwordHasher.Hash(request.Password) != user.PasswordHash)
            throw new UnauthorizedAccessException("Invalid credentials.");

        var token = _tokenService.GenerateToken(user);

        return new AuthResponse
        {
            Email = user.Email,
            Token = token
        };
    }
}