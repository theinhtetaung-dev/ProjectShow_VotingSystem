using VotingSystem.Shared;

namespace VotingSystem.Domain.Features.Role;

public interface IRoleService
{
    Task<Result<RoleResponseModel>> CreateRoleAsync(RoleCreateRequestModel request);
    Task<Result<RoleResponseModel>> DeleteRole(int id);
    Task<Result<RoleResponseModel>> GetRoleByID(int id);
    Task<Result<List<RoleResponseModel>>> GetRolelist();
    Task<Result<RoleResponseModel>> UpdateRole(RoleUpdateRequestModel request);
}
