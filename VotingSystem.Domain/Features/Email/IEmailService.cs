using VotingSystem.Shared;

namespace VotingSystem.Domain.Features.Email;

public interface IEmailService
{
    Task<Result<EmailSendResponseModel>> SendEmailAsync(
        EmailSendRequestModel request,
        CancellationToken cancellationToken = default);
}
