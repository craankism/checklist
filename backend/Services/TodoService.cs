using Checklist.Api.DTOs;
using Checklist.Api.Models;
using Checklist.Api.Repositories;

namespace Checklist.Api.Services;

/// <summary>
/// Handles todo business rules and DTO mapping.
/// </summary>
public class TodoService : ITodoService
{
    private readonly ITodoRepository _todoRepository;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Creates a service with repositories for users and todos.
    /// </summary>
    /// <param name="todoRepository">Todo persistence repository.</param>
    /// <param name="userRepository">User persistence repository.</param>
    public TodoService(ITodoRepository todoRepository, IUserRepository userRepository)
    {
        _todoRepository = todoRepository;
        _userRepository = userRepository;
    }

    /// <inheritdoc />
    public async Task<List<TodoItemReadDto>> GetAllAsync()
    {
        var user = await GetDefaultUserOrThrowAsync();
        var todos = await _todoRepository.GetAllByUserIdAsync(user.Id);
        return todos.Select(MapToReadDto).ToList();
    }

    /// <inheritdoc />
    public async Task<TodoItemReadDto?> GetByIdAsync(int id)
    {
        var user = await GetDefaultUserOrThrowAsync();
        var todo = await _todoRepository.GetByIdAsync(user.Id, id);
        return todo is null ? null : MapToReadDto(todo);
    }

    /// <inheritdoc />
    public async Task<TodoItemReadDto> CreateAsync(TodoItemCreateDto createDto)
    {
        var user = await GetDefaultUserOrThrowAsync();

        var entity = new TodoItem
        {
            UserId = user.Id,
            Title = createDto.Title.Trim(),
            Description = createDto.Description?.Trim(),
            DueDate = createDto.DueDate,
            Priority = createDto.Priority,
            IsDone = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _todoRepository.AddAsync(entity);
        return MapToReadDto(created);
    }

    /// <inheritdoc />
    public async Task<TodoItemReadDto?> UpdateAsync(int id, TodoItemUpdateDto updateDto)
    {
        var user = await GetDefaultUserOrThrowAsync();
        var todo = await _todoRepository.GetByIdAsync(user.Id, id);
        if (todo is null)
        {
            return null;
        }

        todo.Title = updateDto.Title.Trim();
        todo.Description = updateDto.Description?.Trim();
        todo.DueDate = updateDto.DueDate;
        todo.Priority = updateDto.Priority;
        todo.IsDone = updateDto.IsDone;
        todo.UpdatedAt = DateTime.UtcNow;

        await _todoRepository.UpdateAsync(todo);
        return MapToReadDto(todo);
    }

    /// <inheritdoc />
    public async Task<TodoItemReadDto?> ToggleCompleteAsync(int id)
    {
        var user = await GetDefaultUserOrThrowAsync();
        var todo = await _todoRepository.GetByIdAsync(user.Id, id);
        if (todo is null)
        {
            return null;
        }

        todo.IsDone = !todo.IsDone;
        todo.UpdatedAt = DateTime.UtcNow;

        await _todoRepository.UpdateAsync(todo);
        return MapToReadDto(todo);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id)
    {
        var user = await GetDefaultUserOrThrowAsync();
        var todo = await _todoRepository.GetByIdAsync(user.Id, id);
        if (todo is null)
        {
            return false;
        }

        await _todoRepository.DeleteAsync(todo);
        return true;
    }

    /// <summary>
    /// Resolves the default local user or throws a clear startup error if missing.
    /// </summary>
    private async Task<User> GetDefaultUserOrThrowAsync()
    {
        var user = await _userRepository.GetDefaultUserAsync();
        if (user is null)
        {
            throw new InvalidOperationException("Default local user was not found. Ensure database initialization has run.");
        }

        return user;
    }

    /// <summary>
    /// Maps a database entity to API response DTO.
    /// </summary>
    /// <param name="entity">Todo entity from persistence layer.</param>
    private static TodoItemReadDto MapToReadDto(TodoItem entity)
    {
        return new TodoItemReadDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            DueDate = entity.DueDate,
            Priority = entity.Priority,
            IsDone = entity.IsDone,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
