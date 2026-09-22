using Checklist.Api.Models;

namespace Checklist.Api.Repositories;

/// <summary>
/// Repository contract for Google OAuth credential persistence.
/// </summary>
public interface IGoogleOAuthCredentialRepository
{
    /// <summary>
    /// Returns credentials for a given user id.
    /// </summary>
    Task<GoogleOAuthCredential?> GetByUserIdAsync(int userId);

    /// <summary>
    /// Adds new credentials and saves changes.
    /// </summary>
    Task<GoogleOAuthCredential> AddAsync(GoogleOAuthCredential credential);

    /// <summary>
    /// Updates stored credentials and saves changes.
    /// </summary>
    Task UpdateAsync(GoogleOAuthCredential credential);
}
