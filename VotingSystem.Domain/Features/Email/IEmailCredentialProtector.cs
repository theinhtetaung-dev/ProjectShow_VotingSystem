namespace VotingSystem.Domain.Features.Email;

public interface IEmailCredentialProtector
{
    string Protect(string plainSecret);
    string Unprotect(string protectedSecret);
}
