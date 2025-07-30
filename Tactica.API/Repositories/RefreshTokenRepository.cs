using Microsoft.EntityFrameworkCore;
using Tactica.API.Data;
using Tactica.API.Models;
using Tactica.API.Repositories.Interfaces;

namespace Tactica.API.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task SaveAsync(RefreshToken token)
    {
        _context.RefreshTokens.Add(token);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked);
    }

    /// <inheritdoc />
    public async Task RevokeAsync(RefreshToken token)
    {
        token.IsRevoked = true;
        await _context.SaveChangesAsync();
    }
}

