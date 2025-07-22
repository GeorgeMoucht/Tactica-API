using Microsoft.AspNetCore.Identity;
using Tactica.API.Infrastructure.Security;
using Tactica.API.Repositories;
using Tactica.API.Repositories.Interfaces;
using Tactica.API.Services;
using Tactica.API.Services.Interfaces;

namespace Tactica.API.Extensions;

/// <summary>
/// Extension method for configuring application-specific services
/// for dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers core application services and repositories for 
    /// dependency injection.
    /// </summary>
    /// <param name="services">The service collection to which dependencies are added.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPasswordHasher, Sha256PasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();

        return services;
    }
}