namespace Checklist.Api.Models;

/// <summary>
/// Represents a local user profile. The app currently uses one default user,
/// but this entity is already in place to support multi-user expansion later.
/// </summary>
public class User
{
    /// <summary>
    /// Primary key for the user.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Display name shown in UI or logs.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// UTC timestamp for creation auditing.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Navigation property for all todos owned by this user.
    /// </summary>
    public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
}
