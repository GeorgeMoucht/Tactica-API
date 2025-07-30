using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
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
    private readonly IRefreshTokenRepository _refreshTokenRepo;

    public AuthService(
        IUserRepository userRepo,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepo
    )
    {
        _userRepo = userRepo;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _refreshTokenRepo = refreshTokenRepo;
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
        var refreshToken = await GenerateRefreshTokenAsync(newUser);

        return new AuthResponse
        {
            Email = newUser.Email,
            Token = token,
            RefreshToken = refreshToken
        };
    }

    /// <inheritdoc /> 
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepo.GetByEmailAsync(request.Email);
        if (user == null || _passwordHasher.Hash(request.Password) != user.PasswordHash)
            throw new UnauthorizedAccessException("Invalid credentials.");

        var token = _tokenService.GenerateToken(user);
        var refreshToken = await GenerateRefreshTokenAsync(user);

        return new AuthResponse
        {
            Email = user.Email,
            Token = token,
            RefreshToken = refreshToken
        };
    }

    /// <inheritdoc /> 
    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        var tokenRecord = await _refreshTokenRepo.GetByTokenAsync(refreshToken);
        if (tokenRecord == null || tokenRecord.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        await _refreshTokenRepo.RevokeAsync(tokenRecord);

        var newJwt = _tokenService.GenerateToken(tokenRecord.User);
        var newRefreshToken = await GenerateRefreshTokenAsync(tokenRecord.User);

        return new AuthResponse
        {
            Email = tokenRecord.User.Email,
            Token = newJwt,
            RefreshToken = newRefreshToken
        };
    }

    /// <summary>
    /// Generates a secure refresh token, saves it to the database,
    /// and return the token string.
    /// </summary>
    /// <param name="user">The user for whom the refresh token is being generated</param>
    /// <returns>The generated refresh token string.</returns>
    private async Task<string> GenerateRefreshTokenAsync(User user)
    {
        var token = _tokenService.GenerateSecureRefreshToken();

        var refreshToken = new RefreshToken
        {
            Token = token,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        await _refreshTokenRepo.SaveAsync(refreshToken);
        return token;
    }
}