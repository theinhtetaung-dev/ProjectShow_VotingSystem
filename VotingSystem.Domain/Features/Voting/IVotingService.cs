using VotingSystem.Shared;

namespace VotingSystem.Domain.Features.Voting;

public interface IVotingService
{
    Task<Result<List<VotingResponseModel>>> GetVotinglist();
    Task<Result<VoteResponseModel>> VoteAsync(VoteRequestModel request);
    Task<Result<VotingLimitResponseModel>> UpdateVotingLimitAsync(VotingLimitUpdateRequestModel request);
}
