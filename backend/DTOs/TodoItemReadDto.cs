using Checklist.Api.Enums;

namespace Checklist.Api.DTOs;

/// <summary>
/// Output DTO used for todo item responses.
/// </summary>
public class TodoItemReadDto
{
    /// <summary>
    /// Todo identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Todo title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Todo description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Optional due date.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Todo priority.
    /// </summary>
    public TodoPriority Priority { get; set; }

    /// <summary>
    /// Completion state.
    /// </summary>
    public bool IsDone { get; set; }

    /// <summary>
    /// UTC creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// UTC update timestamp.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
