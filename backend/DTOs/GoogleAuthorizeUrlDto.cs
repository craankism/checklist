namespace Checklist.Api.DTOs;

/// <summary>
/// Output DTO containing a Google OAuth authorization URL.
/// </summary>
public class GoogleAuthorizeUrlDto
{
    /// <summary>
    /// URL the frontend opens so the user can grant access.
    /// </summary>
    public string Url { get; set; } = string.Empty;
}
