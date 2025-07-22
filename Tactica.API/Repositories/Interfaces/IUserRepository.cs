using Tactica.API.Models;

namespace Tactica.API.Repositories.Interfaces;

/// <summary>
/// Defines methods for interacting with the User data store.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrives a user by their email address.
    /// </summary>
    /// <param name="email">The user's email.</param>
    /// <returns>The matching <see cref="User"/> or null if not found.</returns>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Adds a new user to the database.
    /// </summary>
    /// <param name="user">The user to add.</param>
    Task AddUserAsync(User user);
}