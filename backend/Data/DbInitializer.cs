using Checklist.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Checklist.Api.Data;

/// <summary>
/// Creates database objects and seeds required local records.
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// Ensures the database exists and inserts one default local user when missing.
    /// </summary>
    /// <param name="dbContext">Application database context.</param>
    public static async Task InitializeAsync(AppDbContext dbContext)
    {
        await dbContext.Database.EnsureCreatedAsync();

        var hasUsers = await dbContext.Users.AnyAsync();
        if (hasUsers)
        {
            return;
        }

        var defaultUser = new User
        {
            DisplayName = "Local User"
        };

        dbContext.Users.Add(defaultUser);
        await dbContext.SaveChangesAsync();
    }
}
