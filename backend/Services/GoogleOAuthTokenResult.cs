namespace Checklist.Api.Services;

/// <summary>
/// Represents token response values returned by Google OAuth token endpoint.
/// </summary>
public class GoogleOAuthTokenResult
{
    /// <summary>
    /// Access token used for API calls.
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Refresh token used to obtain new access tokens.
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Expiry in seconds returned by Google.
    /// </summary>
    public int ExpiresInSeconds { get; set; }
}
