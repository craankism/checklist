using Checklist.Api.DTOs;

namespace Checklist.Api.Services;

/// <summary>
/// Business logic contract for todo operations.
/// </summary>
public interface ITodoService
{
    /// <summary>
    /// Returns all todos for the current local user.
    /// </summary>
    Task<List<TodoItemReadDto>> GetAllAsync();

    /// <summary>
    /// Returns one todo by id for the current local user.
    /// </summary>
    Task<TodoItemReadDto?> GetByIdAsync(int id);

    /// <summary>
    /// Creates a new todo for the current local user.
    /// </summary>
    Task<TodoItemReadDto> CreateAsync(TodoItemCreateDto createDto);

    /// <summary>
    /// Updates one todo for the current local user.
    /// </summary>
    Task<TodoItemReadDto?> UpdateAsync(int id, TodoItemUpdateDto updateDto);

    /// <summary>
    /// Toggles completion for one todo.
    /// </summary>
    Task<TodoItemReadDto?> ToggleCompleteAsync(int id);

    /// <summary>
    /// Deletes one todo.
    /// </summary>
    Task<bool> DeleteAsync(int id);
}
