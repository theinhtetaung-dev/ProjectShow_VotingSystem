namespace VotingSystem.Domain.Features.Result;

public interface IResultService
{
    Task<VotingSystem.Shared.Result<ResultResponseModel>> CreateResultAsync(ResultCreateRequestModel request);
    Task<VotingSystem.Shared.Result<ResultResponseModel>> GetResultByID(long id);
    Task<VotingSystem.Shared.Result<List<ResultResponseModel>>> GetResultlist();
    Task<VotingSystem.Shared.Result<ResultResponseModel>> UpdateResult(ResultUpdateRequestModel request);
}
