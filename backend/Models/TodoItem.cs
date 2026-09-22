using Checklist.Api.Enums;

namespace Checklist.Api.Models;

/// <summary>
/// Represents one task in the checklist.
/// </summary>
public class TodoItem
{
    /// <summary>
    /// Primary key for the todo.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Required short title for quick identification.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Optional long-form text with details.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Optional due date used by reminders and sorting.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Priority level used for ordering and urgency indicators.
    /// </summary>
    public TodoPriority Priority { get; set; } = TodoPriority.Medium;

    /// <summary>
    /// Completion state of this task.
    /// </summary>
    public bool IsDone { get; set; }

    /// <summary>
    /// UTC timestamp when the todo was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// UTC timestamp of the last update.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Foreign key to the owning user.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Navigation property to the owning user.
    /// </summary>
    public User? User { get; set; }
}
