namespace Checklist.Api.Services;

/// <summary>
/// Service contract for Google Calendar actions linked to todo items.
/// </summary>
public interface IGoogleCalendarService
{
    /// <summary>
    /// Creates a calendar reminder event for a specific todo id.
    /// </summary>
    Task<string> CreateReminderForTodoAsync(int todoId);
}
