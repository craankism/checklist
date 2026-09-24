using Checklist.Api.DTOs;
using Checklist.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Checklist.Api.Controllers;

/// <summary>
/// Exposes endpoints for Google Calendar reminder creation.
/// </summary>
[ApiController]
[Route("api/google-calendar")]
public class GoogleCalendarController : ControllerBase
{
    private readonly IGoogleCalendarService _googleCalendarService;

    /// <summary>
    /// Creates a controller with calendar service dependency.
    /// </summary>
    /// <param name="googleCalendarService">Google Calendar service.</param>
    public GoogleCalendarController(IGoogleCalendarService googleCalendarService)
    {
        _googleCalendarService = googleCalendarService;
    }

    /// <summary>
    /// Creates a Google Calendar reminder event for the given todo item.
    /// </summary>
    /// <param name="todoId">Todo identifier.</param>
    [HttpPost("todos/{todoId:int}/reminder")]
    public async Task<ActionResult<GoogleReminderResultDto>> CreateReminderAsync([FromRoute] int todoId)
    {
        var reminderUrl = await _googleCalendarService.CreateReminderForTodoAsync(todoId);
        return Ok(new GoogleReminderResultDto { ReminderUrl = reminderUrl });
    }
}
