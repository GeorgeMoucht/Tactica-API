using Microsoft.AspNetCore.Mvc;
using Tactica.API.DTOs.Auth;
using Tactica.API.Services.Interfaces;

namespace Tactica.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registers a new user with the provided email and password.
    /// </summary>
    /// <param name="request">The registration request containing email and password.</param>
    /// <returns>Returns a 200 OK with a JWT if successful, or 400 Bad Request if email already exists.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new Dictionary<string, string> { {
                "error", ex.Message
            }});
        }
    }

    /// <summary>
    /// Authenticates a user and returns a JWT if credentials are valid.
    /// </summary>
    /// <param name="request">The login request containing email and password.</param>
    /// <returns>Returns 200 OK with token or 401 Unauthorized if credentials are invalid.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new Dictionary<string, string> {{
                "error", "Invalid credentials."
            }});
        }
    }
}