using VotingSystem.Shared;

namespace VotingSystem.Domain.Features.Email;

public interface IEmailAccountSelector
{
    Task<Result<SelectedEmailAccount>> SelectNextAccountAsync(
        IReadOnlyCollection<long>? excludedAccountIds = null,
        CancellationToken cancellationToken = default);
}
