using Checklist.Api.Repositories;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;

namespace Checklist.Api.Services;

/// <summary>
/// Creates Google Calendar reminder events from todo items.
/// </summary>
public class GoogleCalendarService : IGoogleCalendarService
{
    private readonly ITodoRepository _todoRepository;
    private readonly IUserRepository _userRepository;
    private readonly IGoogleOAuthService _googleOAuthService;

    /// <summary>
    /// Creates a service with dependencies needed to fetch todos and call Google Calendar.
    /// </summary>
    /// <param name="todoRepository">Todo repository.</param>
    /// <param name="userRepository">User repository.</param>
    /// <param name="googleOAuthService">Google OAuth service.</param>
    public GoogleCalendarService(
        ITodoRepository todoRepository,
        IUserRepository userRepository,
        IGoogleOAuthService googleOAuthService)
    {
        _todoRepository = todoRepository;
        _userRepository = userRepository;
        _googleOAuthService = googleOAuthService;
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

        var accessToken = await _googleOAuthService.GetValidAccessTokenAsync();

        // Google SDK accepts access tokens through GoogleCredential for authenticated requests.
        var credential = Google.Apis.Auth.OAuth2.GoogleCredential.FromAccessToken(accessToken);
        using var calendarService = new CalendarService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "Checklist Desktop App"
        });

        // Use a short event window around due date so the reminder appears in calendar timelines.
        var dueUtc = DateTime.SpecifyKind(todo.DueDate.Value, DateTimeKind.Local).ToUniversalTime();
        var calendarEvent = new Event
        {
            Summary = todo.Title,
            Description = todo.Description,
            Start = new EventDateTime
            {
                DateTimeDateTimeOffset = new DateTimeOffset(dueUtc),
                TimeZone = "UTC"
            },
            End = new EventDateTime
            {
                DateTimeDateTimeOffset = new DateTimeOffset(dueUtc.AddMinutes(30)),
                TimeZone = "UTC"
            },
            Reminders = new Event.RemindersData
            {
                UseDefault = false,
                Overrides =
                [
                    new EventReminder { Method = "popup", Minutes = 10 }
                ]
            }
        };

        var request = calendarService.Events.Insert(calendarEvent, "primary");
        var created = await request.ExecuteAsync();

        return created.Id ?? string.Empty;
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
