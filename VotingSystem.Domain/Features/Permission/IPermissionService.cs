using VotingSystem.Shared;

namespace VotingSystem.Domain.Features.Permission;

public interface IPermissionService
{
    Task<Result<PermissionResponseModel>> CreatePermissionAsync(PermissionCreateRequestModel request);
    Task<Result<PermissionResponseModel>> DeletePermission(int id);
    Task<Result<PermissionResponseModel>> GetPermissionByID(int id);
    Task<Result<List<PermissionResponseModel>>> GetPermissionlist();
    Task<Result<PermissionResponseModel>> UpdatePermission(PermissionUpdateRequestModel request);
}
