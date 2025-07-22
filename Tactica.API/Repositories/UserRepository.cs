using Microsoft.EntityFrameworkCore;
using Tactica.API.Data;
using Tactica.API.Models;
using Tactica.API.Repositories.Interfaces;

namespace Tactica.API.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(
            u => u.Email == email
        );
    }

    /// <inheritdoc />
    public async Task AddUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
}