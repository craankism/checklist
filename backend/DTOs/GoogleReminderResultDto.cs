namespace Checklist.Api.DTOs;

/// <summary>
/// Output DTO with the created Google Calendar event id.
/// </summary>
public class GoogleReminderResultDto
{
    /// <summary>
    /// Calendar event id returned by Google.
    /// </summary>
    public string EventId { get; set; } = string.Empty;
}
