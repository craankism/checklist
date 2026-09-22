using Checklist.Api.Data;
using Checklist.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Checklist.Api.Repositories;

/// <summary>
/// EF Core implementation for todo repository operations.
/// </summary>
public class TodoRepository : ITodoRepository
{
    private readonly AppDbContext _dbContext;

    /// <summary>
    /// Creates the repository with the injected database context.
    /// </summary>
    /// <param name="dbContext">Application database context.</param>
    public TodoRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<List<TodoItem>> GetAllByUserIdAsync(int userId)
    {
        return await _dbContext.TodoItems
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.IsDone)
            .ThenByDescending(x => x.Priority)
            .ThenBy(x => x.DueDate)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<TodoItem?> GetByIdAsync(int userId, int todoId)
    {
        return await _dbContext.TodoItems
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == todoId);
    }

    /// <inheritdoc />
    public async Task<TodoItem> AddAsync(TodoItem todoItem)
    {
        _dbContext.TodoItems.Add(todoItem);
        await _dbContext.SaveChangesAsync();
        return todoItem;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(TodoItem todoItem)
    {
        _dbContext.TodoItems.Update(todoItem);
        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(TodoItem todoItem)
    {
        _dbContext.TodoItems.Remove(todoItem);
        await _dbContext.SaveChangesAsync();
    }
}
