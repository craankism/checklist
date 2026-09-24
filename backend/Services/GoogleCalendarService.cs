using Checklist.Api.Repositories;
using Microsoft.AspNetCore.WebUtilities;

namespace Checklist.Api.Services;

/// <summary>
/// Creates Google Calendar reminder events from todo items.
/// </summary>
public class GoogleCalendarService : IGoogleCalendarService
{
    private readonly ITodoRepository _todoRepository;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Creates a service with dependencies needed to fetch todo reminder details.
    /// </summary>
    /// <param name="todoRepository">Todo repository.</param>
    /// <param name="userRepository">User repository.</param>
    public GoogleCalendarService(
        ITodoRepository todoRepository,
        IUserRepository userRepository)
    {
        _todoRepository = todoRepository;
        _userRepository = userRepository;
    }

    /// <inheritdoc />
    public async Task<string> CreateReminderForTodoAsync(int todoId)
    {
        var user = await GetDefaultUserOrThrowAsync();
        var todo = await _todoRepository.GetByIdAsync(user.Id, todoId)
            ?? throw new KeyNotFoundException("Todo item was not found.");

        if (todo.DueDate is null)
        {
            throw new InvalidOperationException("Todo item requires a due date before a Google reminder can be created.");
        }

        // Build a Google Calendar event template URL and let browser session auth decide access.
        var dueUtc = DateTime.SpecifyKind(todo.DueDate.Value, DateTimeKind.Local).ToUniversalTime();
        var endUtc = dueUtc.AddMinutes(30);

        var query = new Dictionary<string, string?>
        {
            ["action"] = "TEMPLATE",
            ["text"] = todo.Title,
            ["details"] = todo.Description,
            ["dates"] = $"{dueUtc:yyyyMMdd'T'HHmmss'Z'}/{endUtc:yyyyMMdd'T'HHmmss'Z'}"
        };

        return QueryHelpers.AddQueryString("https://calendar.google.com/calendar/render", query);
    }

    /// <summary>
    /// Returns the default local user.
    /// </summary>
    private async Task<Models.User> GetDefaultUserOrThrowAsync()
    {
        var user = await _userRepository.GetDefaultUserAsync();
        if (user is null)
        {
            throw new InvalidOperationException("Default local user was not found. Ensure database initialization has run.");
        }

        return user;
    }
}
