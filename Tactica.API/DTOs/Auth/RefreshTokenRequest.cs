namespace Tactica.API.DTOs.Auth;


/// <summary>
/// Request for refreshing a JWT using a refresh token.
/// </summary>
public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}