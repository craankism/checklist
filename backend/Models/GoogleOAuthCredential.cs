namespace Checklist.Api.Models;

/// <summary>
/// Stores encrypted OAuth tokens for one local user.
/// </summary>
public class GoogleOAuthCredential
{
    /// <summary>
    /// Primary key for credentials.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// FK to the owning user.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Encrypted access token stored using ASP.NET Data Protection.
    /// </summary>
    public string EncryptedAccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Encrypted refresh token stored using ASP.NET Data Protection.
    /// </summary>
    public string EncryptedRefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Expiry time for the current access token.
    /// </summary>
    public DateTime AccessTokenExpiresAtUtc { get; set; }

    /// <summary>
    /// UTC timestamp for the last token update.
    /// </summary>
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Navigation back to the owning user.
    /// </summary>
    public User? User { get; set; }
}
