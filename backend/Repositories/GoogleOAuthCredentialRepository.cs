using Checklist.Api.Data;
using Checklist.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Checklist.Api.Repositories;

/// <summary>
/// EF Core implementation for Google OAuth credential operations.
/// </summary>
public class GoogleOAuthCredentialRepository : IGoogleOAuthCredentialRepository
{
    private readonly AppDbContext _dbContext;

    /// <summary>
    /// Creates the repository with the injected context.
    /// </summary>
    /// <param name="dbContext">Application database context.</param>
    public GoogleOAuthCredentialRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<GoogleOAuthCredential?> GetByUserIdAsync(int userId)
    {
        return await _dbContext.GoogleOAuthCredentials
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    /// <inheritdoc />
    public async Task<GoogleOAuthCredential> AddAsync(GoogleOAuthCredential credential)
    {
        _dbContext.GoogleOAuthCredentials.Add(credential);
        await _dbContext.SaveChangesAsync();
        return credential;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(GoogleOAuthCredential credential)
    {
        _dbContext.GoogleOAuthCredentials.Update(credential);
        await _dbContext.SaveChangesAsync();
    }
}
