namespace Checklist.Api.Services;

/// <summary>
/// Service contract for Google OAuth connection flow.
/// </summary>
public interface IGoogleOAuthService
{
    /// <summary>
    /// Builds the authorization URL used by the frontend to start Google OAuth.
    /// </summary>
    Task<string> BuildAuthorizationUrlAsync();

    /// <summary>
    /// Handles OAuth callback and stores encrypted tokens.
    /// </summary>
    Task HandleCallbackAsync(string code);

    /// <summary>
    /// Indicates whether the current local user has connected Google.
    /// </summary>
    Task<bool> IsConnectedAsync();

    /// <summary>
    /// Returns a valid access token for Google API requests.
    /// </summary>
    Task<string> GetValidAccessTokenAsync();
}
