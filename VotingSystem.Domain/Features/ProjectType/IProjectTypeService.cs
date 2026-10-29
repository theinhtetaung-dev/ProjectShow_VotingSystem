using VotingSystem.Shared;

namespace VotingSystem.Domain.Features.ProjectType;

public interface IProjectTypeService
{
    Task<Result<ProjectTypeResponseModel>> CreateProjectTypeAsync(ProjectTypeCreateRequestModel request);
    Task<Result<ProjectTypeResponseModel>> DeleteProjectType(int id);
    Task<Result<ProjectTypeResponseModel>> GetProjectTypeByID(int id);
    Task<Result<List<ProjectTypeResponseModel>>> GetProjectTypelist();
    Task<Result<ProjectTypeResponseModel>> UpdateProjectType(ProjectTypeUpdateRequestModel request);
}
