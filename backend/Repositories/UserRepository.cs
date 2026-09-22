using Checklist.Api.Data;
using Checklist.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Checklist.Api.Repositories;

/// <summary>
/// EF Core implementation for user repository operations.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    /// <summary>
    /// Creates a repository with the injected application context.
    /// </summary>
    /// <param name="dbContext">Database context.</param>
    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<User?> GetDefaultUserAsync()
    {
        return await _dbContext.Users.OrderBy(x => x.Id).FirstOrDefaultAsync();
    }
}
