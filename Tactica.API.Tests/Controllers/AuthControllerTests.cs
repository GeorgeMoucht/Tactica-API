using Moq;
using Microsoft.AspNetCore.Mvc;
using Tactica.API.Controllers;
using Tactica.API.DTOs.Auth;
using Tactica.API.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Tactica.API.Tests.Controllers;

/// <summary>
/// Unit tests for the AuthController, validating HTTP response behavior and service interactions.
/// </summary>
public class AuthControllerTests
{
    /// <summary>
    /// Ensures that a successful registration returns 200 OK with the correct
    /// auth response.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Register_ReturnsOk_WhenSuccessful()
    {
        var mockService = new Mock<IAuthService>();
        var request = new RegisterRequest { Email = "test@example.com", Password = "secure123" };

        var expected = new AuthResponse
        {
            Email = request.Email,
            Token = "token123",
            RefreshToken = "refresh123"
        };

        mockService.Setup(s => s.RegisterAsync(request)).ReturnsAsync(expected);

        var controller = new AuthController(mockService.Object);
        var result = await controller.Register(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsType<AuthResponse>(okResult.Value);

        Assert.Equal(request.Email, data.Email);
        Assert.Equal(expected.Token, data.Token);
        Assert.Equal(expected.RefreshToken, data.RefreshToken);
    }

    /// <summary>
    /// Ensure that attempting to register with an already used email
    /// returns 400 BadRequest.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Register_ReturnsBadRequest_WhenEmailExists()
    {
        var mockService = new Mock<IAuthService>();
        var request = new RegisterRequest { Email = "duplicate@example.com", Password = "pass123" };

        mockService.Setup(s => s.RegisterAsync(request))
            .ThrowsAsync(new InvalidOperationException("Email already in use."));

        var controller = new AuthController(mockService.Object);
        var result = await controller.Register(request);

        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badResult.Value);

        var payload = Assert.IsType<Dictionary<string, string>>(badResult.Value);
        Assert.Equal("Email already in use.", payload["error"]);
    }


    /// <summary>
    /// Ensures that a valid login returns 200 OK
    /// and a JWT token in the response.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Login_ReturnsOk_WhenSuccessful()
    {
        var mockService = new Mock<IAuthService>();
        var request = new LoginRequest { Email = "user@example.com", Password = "123456" };
        var expected = new AuthResponse { Email = request.Email, Token = "jwt" };

        mockService.Setup(s => s.LoginAsync(request)).ReturnsAsync(expected);

        var controller = new AuthController(mockService.Object);
        var result = await controller.Login(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsType<AuthResponse>(okResult.Value);
        Assert.Equal(expected.Email, data.Email);
    }

    /// <summary>
    /// Ensures that an invalid login attempt returns 401 Unauthorized with
    /// an error message.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenCredentialsInvalid()
    {
        var mockService = new Mock<IAuthService>();
        var request = new LoginRequest { Email = "user@example.com", Password = "wrongpass" };

        mockService.Setup(s => s.LoginAsync(request))
            .ThrowsAsync(new UnauthorizedAccessException());

        var controller = new AuthController(mockService.Object);
        var result = await controller.Login(request);

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorized.Value);

        var payload = Assert.IsType<Dictionary<string, string>>(unauthorized.Value);
        Assert.Equal("Invalid credentials.", payload["error"]);
    }

    /// <summary>
    /// Ensures that a valid refresh token returns new tokens.
    /// </summary>
    [Fact]
    public async Task Refresh_ReturnsOk_WhenTokenIsValid()
    {
        var mockService = new Mock<IAuthService>();
        var request = new RefreshTokenRequest { RefreshToken = "valid-refresh-token" };

        var expected = new AuthResponse
        {
            Email = "user@example.com",
            Token = "new-access-token",
            RefreshToken = "new-refresh-token"
        };

        mockService.Setup(s => s.RefreshTokenAsync(request.RefreshToken))
            .ReturnsAsync(expected);

        var controller = new AuthController(mockService.Object);
        var result = await controller.Refresh(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsType<AuthResponse>(okResult.Value);

        Assert.Equal(expected.Email, data.Email);
        Assert.Equal(expected.Token, data.Token);
        Assert.Equal(expected.RefreshToken, data.RefreshToken);
    }

    /// <summary>
    /// Ensures that an invalid or expired refresh token returns 401 Unauthorized.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Refresh_ReturnsUnauthorized_WhenTokenIsInvalid()
    {
        var mockService = new Mock<IAuthService>();
        var request = new RefreshTokenRequest { RefreshToken = "invalid-refresh-token" };

        mockService.Setup(s => s.RefreshTokenAsync(request.RefreshToken))
            .ThrowsAsync(new UnauthorizedAccessException("Invalid or expired refresh token."));

        var controller = new AuthController(mockService.Object);
        var result = await controller.Refresh(request);

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        var payload = Assert.IsType<Dictionary<string, string>>(unauthorized.Value);
        Assert.Equal("Invalid or expired refresh token.", payload["error"]);
    }
}
