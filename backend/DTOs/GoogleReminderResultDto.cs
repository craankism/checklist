namespace Checklist.Api.DTOs;

/// <summary>
/// Output DTO with the Google Calendar reminder creation URL.
/// </summary>
public class GoogleReminderResultDto
{
    /// <summary>
    /// Browser URL that opens a prefilled Google Calendar reminder form.
    /// </summary>
    public string ReminderUrl { get; set; } = string.Empty;
}
