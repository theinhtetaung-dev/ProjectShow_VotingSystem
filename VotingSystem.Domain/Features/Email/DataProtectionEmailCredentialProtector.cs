using Microsoft.AspNetCore.DataProtection;

namespace VotingSystem.Domain.Features.Email;

public sealed class DataProtectionEmailCredentialProtector : IEmailCredentialProtector
{
    private const string Purpose = "VotingSystem.EmailAccounts.AppPassword.v1";
    private readonly IDataProtector _protector;

    public DataProtectionEmailCredentialProtector(IDataProtectionProvider dataProtectionProvider)
    {
        _protector = dataProtectionProvider.CreateProtector(Purpose);
    }

    public string Protect(string plainSecret)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(plainSecret);
        return _protector.Protect(plainSecret);
    }

    public string Unprotect(string protectedSecret)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(protectedSecret);
        return _protector.Unprotect(protectedSecret);
    }
}
