namespace Checklist.Api.DTOs;

/// <summary>
/// Output DTO indicating whether Google OAuth is currently connected.
/// </summary>
public class GoogleAuthStatusDto
{
    /// <summary>
    /// True when stored credentials exist for the local user.
    /// </summary>
    public bool IsConnected { get; set; }
}
