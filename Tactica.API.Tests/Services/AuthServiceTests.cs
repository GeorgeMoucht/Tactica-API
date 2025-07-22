using Moq;
using Tactica.API.Models;
using Tactica.API.DTOs.Auth;
using Tactica.API.Services;
using Tactica.API.Repositories.Interfaces;
using Tactica.API.Infrastructure.Security;

namespace Tactica.API.Tests.Services;

/// <summary>
/// Unit tests for the AuthService class, which handles user authentication logic.
/// </summary>
public class AuthServiceTest
{
    private readonly Mock<IUserRepository> _mockRepo = new();
    private readonly Mock<IPasswordHasher> _mockHasher = new();
    private readonly Mock<ITokenService> _mockToken = new();

    private AuthService CreateService() =>
        new AuthService(_mockRepo.Object, _mockHasher.Object, _mockToken.Object);

    /// <summary>
    /// Verifies that RegisterAsync throws an InvalidOperationException
    /// when the email is already in use.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterAsync_Throws_WhenEmailAlreadyExists()
    {
        _mockRepo.Setup(r => r.GetByEmailAsync("taken@example.com"))
                 .ReturnsAsync(new User { Email = "taken@example.com" });

        var service = CreateService();

        var request = new RegisterRequest
        {
            Email = "taken@example.com",
            Password = "123456"
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.RegisterAsync(request));
    }

    /// <summary>
    /// Verifies that RegisterAsync returns a token when registration is
    /// successful.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RegisterAsync_Succeeds_AndReturnsToken()
    {
        _mockRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                 .ReturnsAsync((User?)null);

        _mockHasher.Setup(h => h.Hash("securepass"))
                   .Returns("hashed-password");

        _mockToken.Setup(t => t.GenerateToken(It.IsAny<User>()))
                  .Returns("mock-token");

        _mockRepo.Setup(r => r.AddUserAsync(It.IsAny<User>()))
                 .Returns(Task.CompletedTask);

        var service = CreateService();

        var request = new RegisterRequest
        {
            Email = "newuser@example.com",
            Password = "securepass"
        };

        var result = await service.RegisterAsync(request);

        Assert.Equal(request.Email, result.Email);
        Assert.Equal("mock-token", result.Token);
    }

    /// <summary>
    /// Verifies that LoginAsync throws an UnauthorizedAccessException
    /// when no user is found with given email.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task LoginAsync_Throws_WhenUserNotFound()
    {
        _mockRepo.Setup(r => r.GetByEmailAsync("missing@example.com"))
                 .ReturnsAsync((User?)null);

        var service = CreateService();

        var request = new LoginRequest
        {
            Email = "missing@example.com",
            Password = "anything"
        };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.LoginAsync(request));
    }


    /// <summary>
    /// Verifies that LoginAsync throws an UnauthorizedAccessException
    /// when the provided password is incorrect.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task LoginAsync_Throws_WhenPasswordIncorrect()
    {
        var request = new LoginRequest
        {
            Email = "user@example.com",
            Password = "wrongpass"
        };

        var user = new User
        {
            Email = request.Email,
            PasswordHash = "correct-hash"
        };

        _mockRepo.Setup(r => r.GetByEmailAsync(request.Email))
                 .ReturnsAsync(user);

        _mockHasher.Setup(h => h.Hash(request.Password))
                   .Returns("wrong-hash");

        var service = CreateService();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.LoginAsync(request));
    }

    /// <summary>
    /// Verifies that LoginAsync succeeds and returns a token
    /// when the credentials are valid.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task LoginAsync_Succeeds_AndReturnsToken()
    {
        var request = new LoginRequest
        {
            Email = "user@example.com",
            Password = "correctpass"
        };

        var user = new User
        {
            Id = 1,
            Email = request.Email,
            PasswordHash = "correct-hash"
        };

        _mockRepo.Setup(r => r.GetByEmailAsync(request.Email))
                 .ReturnsAsync(user);

        _mockHasher.Setup(h => h.Hash(request.Password))
                   .Returns("correct-hash");

        _mockToken.Setup(t => t.GenerateToken(user))
                  .Returns("valid-token");

        var service = CreateService();

        var result = await service.LoginAsync(request);

        Assert.Equal(request.Email, result.Email);
        Assert.Equal("valid-token", result.Token);
    }
}
