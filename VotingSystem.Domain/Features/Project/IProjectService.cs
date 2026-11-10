using VotingSystem.Shared;

namespace VotingSystem.Domain.Features.Project;

public interface IProjectService
{
    Task<Result<ProjectResponseModel>> CreateProjectAsync(ProjectCreateRequestModel request);
    Task<Result<ProjectResponseModel>> DeleteProject(long id, long? eventId = null);
    Task<Result<ProjectResponseModel>> GetProjectByID(long id);
    Task<Result<PageResult<ProjectResponseModel>>> GetProjectlist(int pageNumber, int pageSize, string? status);
    Task<Result<ProjectResponseModel>> UpdateProject(ProjectUpdateRequestModel request);
}
