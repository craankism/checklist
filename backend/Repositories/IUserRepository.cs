using Checklist.Api.Models;

namespace Checklist.Api.Repositories;

/// <summary>
/// Repository contract for user operations.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Returns the first local user used by this base application.
    /// </summary>
    Task<User?> GetDefaultUserAsync();
}
