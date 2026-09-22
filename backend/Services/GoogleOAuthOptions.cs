namespace Checklist.Api.Services;

/// <summary>
/// Configuration object loaded from app settings for Google OAuth.
/// </summary>
public class GoogleOAuthOptions
{
    /// <summary>
    /// OAuth client id from Google Cloud Console.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// OAuth client secret from Google Cloud Console.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Redirect URI registered in Google Cloud Console.
    /// </summary>
    public string RedirectUri { get; set; } = string.Empty;
}
