using VotingSystem.Shared;

namespace VotingSystem.Domain.Features.Email;

public interface IEmailAccountService
{
    Task<Result<List<EmailAccountResponseModel>>> GetEmailAccountsAsync(
        bool? isActive = null,
        CancellationToken cancellationToken = default);

    Task<Result<EmailAccountResponseModel>> GetEmailAccountByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result<EmailAccountResponseModel>> CreateEmailAccountAsync(
        EmailAccountCreateRequestModel request,
        CancellationToken cancellationToken = default);

    Task<Result<EmailAccountResponseModel>> UpdateEmailAccountAsync(
        EmailAccountUpdateRequestModel request,
        CancellationToken cancellationToken = default);

    Task<Result<EmailAccountResponseModel>> SetEmailAccountStatusAsync(
        long id,
        bool isActive,
        CancellationToken cancellationToken = default);

    Task<Result<EmailAccountResponseModel>> DeleteEmailAccountAsync(
        long id,
        CancellationToken cancellationToken = default);
}
