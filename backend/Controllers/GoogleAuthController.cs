using Checklist.Api.DTOs;
using Checklist.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Checklist.Api.Controllers;

/// <summary>
/// Exposes Google OAuth endpoints for connect flow.
/// </summary>
[ApiController]
[Route("api/google-auth")]
public class GoogleAuthController : ControllerBase
{
    private readonly IGoogleOAuthService _googleOAuthService;

    /// <summary>
    /// Creates a controller with Google OAuth service.
    /// </summary>
    /// <param name="googleOAuthService">Google OAuth service.</param>
    public GoogleAuthController(IGoogleOAuthService googleOAuthService)
    {
        _googleOAuthService = googleOAuthService;
    }

    /// <summary>
    /// Returns an authorization URL that the frontend opens in the system browser.
    /// </summary>
    [HttpGet("authorize-url")]
    public async Task<ActionResult<GoogleAuthorizeUrlDto>> GetAuthorizeUrlAsync()
    {
        var url = await _googleOAuthService.BuildAuthorizationUrlAsync();
        return Ok(new GoogleAuthorizeUrlDto { Url = url });
    }

    /// <summary>
    /// Handles Google OAuth callback and stores tokens.
    /// </summary>
    /// <param name="code">OAuth authorization code.</param>
    [HttpGet("callback")]
    public async Task<ActionResult> CallbackAsync([FromQuery] string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return BadRequest("Missing OAuth code.");
        }

        await _googleOAuthService.HandleCallbackAsync(code);
        return Content("Google account connected. You can close this tab and return to the desktop app.", "text/plain");
    }

    /// <summary>
    /// Returns connection state for the local user.
    /// </summary>
    [HttpGet("status")]
    public async Task<ActionResult<GoogleAuthStatusDto>> GetStatusAsync()
    {
        var connected = await _googleOAuthService.IsConnectedAsync();
        return Ok(new GoogleAuthStatusDto { IsConnected = connected });
    }
}
