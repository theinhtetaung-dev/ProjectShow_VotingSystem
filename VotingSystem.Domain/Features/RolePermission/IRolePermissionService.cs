using VotingSystem.Shared;

namespace VotingSystem.Domain.Features.RolePermission;

public interface IRolePermissionService
{
    Task<Result<RolePermissionResponseModel>> AssignPermissionToRoleAsync(AssignPermissionToRoleRequestModel request);
    Task<Result<RolePermissionResponseModel>> GetRolePermissionByID(long id);
    Task<Result<List<RolePermissionResponseModel>>> GetRolePermissionlist();
    Task<Result<List<RolePermissionResponseModel>>> GetRolePermissions(int roleId);
    Task<Result<RolePermissionResponseModel>> RemovePermissionFromRoleAsync(RemovePermissionFromRoleRequestModel request);
}
