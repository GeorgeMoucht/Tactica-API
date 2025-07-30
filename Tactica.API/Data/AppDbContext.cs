using Microsoft.EntityFrameworkCore;
using Tactica.API.Models;

namespace Tactica.API.Data;

/// <summary>
/// The application's database context, representing the session with 
/// the database.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext" /> class using specified options. 
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /// <summary>
    /// Represents the Users table in the database.
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Represent a refresh token table in the database.
    /// </summary>
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
}