namespace Checklist.Api.Services;

/// <summary>
/// Provides encryption and decryption helpers for sensitive local token values.
/// </summary>
public interface ITokenProtectionService
{
    /// <summary>
    /// Protects a plaintext token before it is persisted.
    /// </summary>
    string Protect(string plainText);

    /// <summary>
    /// Restores a plaintext token from protected storage.
    /// </summary>
    string Unprotect(string protectedText);
}
