using System.Text.Json;
using Checklist.Api.Models;
using Checklist.Api.Repositories;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Checklist.Api.Services;

/// <summary>
/// Implements Google OAuth authorization code flow for local desktop usage.
/// </summary>
public class GoogleOAuthService : IGoogleOAuthService
{
    private const string GoogleAuthorizeEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
    private const string GoogleTokenEndpoint = "https://oauth2.googleapis.com/token";
    private static readonly string[] Scopes =
    [
        "openid",
        "email",
        "profile",
        "https://www.googleapis.com/auth/calendar.events"
    ];

    private readonly IUserRepository _userRepository;
    private readonly IGoogleOAuthCredentialRepository _credentialRepository;
    private readonly ITokenProtectionService _tokenProtectionService;
    private readonly GoogleOAuthOptions _options;

    /// <summary>
    /// Creates the service with repositories, token protection, and OAuth settings.
    /// </summary>
    /// <param name="userRepository">User repository.</param>
    /// <param name="credentialRepository">Credential repository.</param>
    /// <param name="tokenProtectionService">Token encryption service.</param>
    /// <param name="options">Google OAuth options from configuration.</param>
    public GoogleOAuthService(
        IUserRepository userRepository,
        IGoogleOAuthCredentialRepository credentialRepository,
        ITokenProtectionService tokenProtectionService,
        IOptions<GoogleOAuthOptions> options)
    {
        _userRepository = userRepository;
        _credentialRepository = credentialRepository;
        _tokenProtectionService = tokenProtectionService;
        _options = options.Value;
    }

    /// <inheritdoc />
    public async Task<string> BuildAuthorizationUrlAsync()
    {
        ValidateOAuthConfiguration();

        var queryParams = new Dictionary<string, string?>
        {
            ["client_id"] = _options.ClientId,
            ["redirect_uri"] = _options.RedirectUri,
            ["response_type"] = "code",
            ["scope"] = string.Join(' ', Scopes),
            ["access_type"] = "offline",
            ["prompt"] = "consent",
            ["include_granted_scopes"] = "true"
        };

        return QueryHelpers.AddQueryString(GoogleAuthorizeEndpoint, queryParams);
    }

    /// <inheritdoc />
    public async Task HandleCallbackAsync(string code)
    {
        ValidateOAuthConfiguration();

        var user = await GetDefaultUserOrThrowAsync();
        var tokenResult = await ExchangeCodeForTokenAsync(code);

        var existingCredential = await _credentialRepository.GetByUserIdAsync(user.Id);
        var encryptedAccessToken = _tokenProtectionService.Protect(tokenResult.AccessToken);
        var encryptedRefreshToken = _tokenProtectionService.Protect(tokenResult.RefreshToken);
        var expiresAt = DateTime.UtcNow.AddSeconds(tokenResult.ExpiresInSeconds);

        if (existingCredential is null)
        {
            var credential = new GoogleOAuthCredential
            {
                UserId = user.Id,
                EncryptedAccessToken = encryptedAccessToken,
                EncryptedRefreshToken = encryptedRefreshToken,
                AccessTokenExpiresAtUtc = expiresAt,
                UpdatedAtUtc = DateTime.UtcNow
            };

            await _credentialRepository.AddAsync(credential);
            return;
        }

        existingCredential.EncryptedAccessToken = encryptedAccessToken;
        existingCredential.EncryptedRefreshToken = encryptedRefreshToken;
        existingCredential.AccessTokenExpiresAtUtc = expiresAt;
        existingCredential.UpdatedAtUtc = DateTime.UtcNow;
        await _credentialRepository.UpdateAsync(existingCredential);
    }

    /// <inheritdoc />
    public async Task<bool> IsConnectedAsync()
    {
        var user = await GetDefaultUserOrThrowAsync();
        var credential = await _credentialRepository.GetByUserIdAsync(user.Id);
        return credential is not null;
    }

    /// <inheritdoc />
    public async Task<string> GetValidAccessTokenAsync()
    {
        ValidateOAuthConfiguration();

        var user = await GetDefaultUserOrThrowAsync();
        var credential = await _credentialRepository.GetByUserIdAsync(user.Id)
            ?? throw new InvalidOperationException("Google account is not connected. Connect first before creating reminders.");

        // Reuse current token if still valid for at least 60 seconds.
        if (credential.AccessTokenExpiresAtUtc > DateTime.UtcNow.AddSeconds(60))
        {
            return _tokenProtectionService.Unprotect(credential.EncryptedAccessToken);
        }

        var refreshToken = _tokenProtectionService.Unprotect(credential.EncryptedRefreshToken);
        var refreshed = await RefreshAccessTokenAsync(refreshToken);

        credential.EncryptedAccessToken = _tokenProtectionService.Protect(refreshed.AccessToken);
        credential.AccessTokenExpiresAtUtc = DateTime.UtcNow.AddSeconds(refreshed.ExpiresInSeconds);
        credential.UpdatedAtUtc = DateTime.UtcNow;
        await _credentialRepository.UpdateAsync(credential);

        return refreshed.AccessToken;
    }

    /// <summary>
    /// Exchanges an OAuth authorization code for access and refresh tokens.
    /// </summary>
    /// <param name="code">Authorization code returned by Google.</param>
    private async Task<GoogleOAuthTokenResult> ExchangeCodeForTokenAsync(string code)
    {
        var payload = new Dictionary<string, string>
        {
            ["code"] = code,
            ["client_id"] = _options.ClientId,
            ["client_secret"] = _options.ClientSecret,
            ["redirect_uri"] = _options.RedirectUri,
            ["grant_type"] = "authorization_code"
        };

        using var httpClient = new HttpClient();
        using var response = await httpClient.PostAsync(GoogleTokenEndpoint, new FormUrlEncodedContent(payload));
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Google token exchange failed: {responseBody}");
        }

        var json = JsonDocument.Parse(responseBody).RootElement;

        var accessToken = json.GetProperty("access_token").GetString();
        var refreshToken = json.TryGetProperty("refresh_token", out var refreshTokenProperty)
            ? refreshTokenProperty.GetString()
            : null;
        var expiresIn = json.GetProperty("expires_in").GetInt32();

        if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new InvalidOperationException("Google did not return required access/refresh tokens. Ensure prompt=consent is used.");
        }

        return new GoogleOAuthTokenResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresInSeconds = expiresIn
        };
    }

    /// <summary>
    /// Refreshes an expired access token using the existing refresh token.
    /// </summary>
    /// <param name="refreshToken">Stored refresh token.</param>
    private async Task<GoogleOAuthTokenResult> RefreshAccessTokenAsync(string refreshToken)
    {
        var payload = new Dictionary<string, string>
        {
            ["client_id"] = _options.ClientId,
            ["client_secret"] = _options.ClientSecret,
            ["refresh_token"] = refreshToken,
            ["grant_type"] = "refresh_token"
        };

        using var httpClient = new HttpClient();
        using var response = await httpClient.PostAsync(GoogleTokenEndpoint, new FormUrlEncodedContent(payload));
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Google token refresh failed: {responseBody}");
        }

        var json = JsonDocument.Parse(responseBody).RootElement;
        var accessToken = json.GetProperty("access_token").GetString();
        var expiresIn = json.GetProperty("expires_in").GetInt32();

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new InvalidOperationException("Google refresh response did not include a valid access token.");
        }

        return new GoogleOAuthTokenResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresInSeconds = expiresIn
        };
    }

    /// <summary>
    /// Returns the app's default local user.
    /// </summary>
    private async Task<User> GetDefaultUserOrThrowAsync()
    {
        var user = await _userRepository.GetDefaultUserAsync();
        if (user is null)
        {
            throw new InvalidOperationException("Default local user was not found. Ensure database initialization has run.");
        }

        return user;
    }

    /// <summary>
    /// Validates required OAuth configuration values before network calls.
    /// </summary>
    private void ValidateOAuthConfiguration()
    {
        if (string.IsNullOrWhiteSpace(_options.ClientId) ||
            string.IsNullOrWhiteSpace(_options.ClientSecret) ||
            string.IsNullOrWhiteSpace(_options.RedirectUri))
        {
            throw new InvalidOperationException("Google OAuth is not configured. Set GoogleOAuth values in appsettings.Development.json.");
        }
    }
}
