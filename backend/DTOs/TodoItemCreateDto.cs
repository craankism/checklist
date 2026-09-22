using System.ComponentModel.DataAnnotations;
using Checklist.Api.Enums;

namespace Checklist.Api.DTOs;

/// <summary>
/// Input DTO for creating a todo item.
/// </summary>
public class TodoItemCreateDto
{
    /// <summary>
    /// Required title for the new todo.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Optional description for additional context.
    /// </summary>
    [StringLength(4000)]
    public string? Description { get; set; }

    /// <summary>
    /// Optional due date to use for reminders.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Priority value selected by the user.
    /// </summary>
    public TodoPriority Priority { get; set; } = TodoPriority.Medium;
}
