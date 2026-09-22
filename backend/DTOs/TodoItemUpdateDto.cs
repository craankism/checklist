using System.ComponentModel.DataAnnotations;
using Checklist.Api.Enums;

namespace Checklist.Api.DTOs;

/// <summary>
/// Input DTO for updating an existing todo item.
/// </summary>
public class TodoItemUpdateDto
{
    /// <summary>
    /// Required title for the todo.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Optional description text.
    /// </summary>
    [StringLength(4000)]
    public string? Description { get; set; }

    /// <summary>
    /// Optional due date.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Priority value.
    /// </summary>
    public TodoPriority Priority { get; set; }

    /// <summary>
    /// Updated completion state.
    /// </summary>
    public bool IsDone { get; set; }
}
