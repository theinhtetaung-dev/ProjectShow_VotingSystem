using VotingSystem.Shared;

namespace VotingSystem.Domain.Features.Project;

public interface IProjectService
{
    Task<Result<ProjectResponseModel>> CreateProjectAsync(ProjectCreateRequestModel request);
    Task<Result<ProjectResponseModel>> DeleteProject(long id);
    Task<Result<ProjectResponseModel>> GetProjectByID(long id);
    Task<Result<List<ProjectResponseModel>>> GetProjectlist();
    Task<Result<ProjectResponseModel>> UpdateProject(ProjectUpdateRequestModel request);
}
