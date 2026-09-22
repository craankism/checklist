using Microsoft.AspNetCore.DataProtection;

namespace Checklist.Api.Services;

/// <summary>
/// Uses ASP.NET Core Data Protection to encrypt token values in local storage.
/// Tradeoff: this is simple and cross-platform, but key management remains local
/// to the machine profile rather than using an enterprise secret vault.
/// </summary>
public class TokenProtectionService : ITokenProtectionService
{
    private readonly IDataProtector _dataProtector;

    /// <summary>
    /// Creates a purpose-specific protector for OAuth token values.
    /// </summary>
    /// <param name="dataProtectionProvider">Framework data protection provider.</param>
    public TokenProtectionService(IDataProtectionProvider dataProtectionProvider)
    {
        _dataProtector = dataProtectionProvider.CreateProtector("ChecklistDesktop.GoogleOAuthTokens.v1");
    }

    /// <inheritdoc />
    public string Protect(string plainText)
    {
        return _dataProtector.Protect(plainText);
    }

    /// <inheritdoc />
    public string Unprotect(string protectedText)
    {
        return _dataProtector.Unprotect(protectedText);
    }
}
