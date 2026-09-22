using Checklist.Api.Models;

namespace Checklist.Api.Repositories;

/// <summary>
/// Repository contract for todo persistence operations.
/// </summary>
public interface ITodoRepository
{
    /// <summary>
    /// Returns all todo items for a given user.
    /// </summary>
    Task<List<TodoItem>> GetAllByUserIdAsync(int userId);

    /// <summary>
    /// Returns one todo by id and user id.
    /// </summary>
    Task<TodoItem?> GetByIdAsync(int userId, int todoId);

    /// <summary>
    /// Adds and saves a new todo.
    /// </summary>
    Task<TodoItem> AddAsync(TodoItem todoItem);

    /// <summary>
    /// Saves an existing todo after changes.
    /// </summary>
    Task UpdateAsync(TodoItem todoItem);

    /// <summary>
    /// Deletes one todo item.
    /// </summary>
    Task DeleteAsync(TodoItem todoItem);
}
