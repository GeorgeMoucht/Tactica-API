namespace Tactica.API.Configuration;

/// <summary>
/// Represents configuration settings for JSON Web Token (JWT) generation
/// and validation.
/// Bound from the 'JwtSettings' section in appsettings.json
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Secret key used to sign to JWT.
    /// </summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// Expected issuer of the JWT.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Expected audience for the JWT.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Token expiration time in minutes.
    /// </summary>
    public int ExpiryMinutes { get; set; }
}