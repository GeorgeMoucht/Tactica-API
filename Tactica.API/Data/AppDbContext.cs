using Microsoft.EntityFrameworkCore;

namespace Tactica.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Temporary dummy DbSet to test connection. (later will be deleted)
    public DbSet<TempConnectionTest> TempConnectionTest => Set<TempConnectionTest>();
}

public class TempConnectionTest
{
    public int Id { get; set; }
}