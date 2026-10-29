using Microsoft.EntityFrameworkCore;
using VotingSystem.DataAccess.AppDbContextModels;
using VotingSystem.Shared;

namespace VotingSystem.Domain.Features.RolePermission;

public class RolePermissionService : IRolePermissionService
{
    private readonly AppDbContext _dbContext;

    public RolePermissionService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region Get Role Permission List
    public async Task<Result<List<RolePermissionResponseModel>>> GetRolePermissionlist()
    {
        try
        {
            var rolePermissions = await _dbContext.TblRolePermissions
                .Include(x => x.Role)
                .Include(x => x.Permission)
                .Where(x => !x.DeleteFlag)
                .ToListAsync();
            var resModel = MapRolePermissions(rolePermissions);

            string message = rolePermissions.Count == 0 ? "No role permissions found." : "Success";
            return Result<List<RolePermissionResponseModel>>.Success(resModel, message);
        }
        catch (Exception ex)
        {
            return Result<List<RolePermissionResponseModel>>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Get Role Permission By ID
    public async Task<Result<RolePermissionResponseModel>> GetRolePermissionByID(long id)
    {
        try
        {
            var rolePermission = await _dbContext.TblRolePermissions
                .Include(x => x.Role)
                .Include(x => x.Permission)
                .FirstOrDefaultAsync(x => x.Id == id && !x.DeleteFlag);
            if (rolePermission == null)
            {
                return Result<RolePermissionResponseModel>.NotFound("Item Not Found!");
            }

            var resModel = MapRolePermission(rolePermission);
            return Result<RolePermissionResponseModel>.Success(resModel, "Success");
        }
        catch (Exception ex)
        {
            return Result<RolePermissionResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Get Permissions By Role ID
    public async Task<Result<List<RolePermissionResponseModel>>> GetRolePermissions(int roleId)
    {
        try
        {
            if (!roleId.IsPositive())
            {
                return Result<List<RolePermissionResponseModel>>.ValidationError("Role ID must be greater than zero.");
            }

            var roleExists = await _dbContext.TblRoles
                .AnyAsync(x => x.Id == roleId && !x.DeleteFlag);
            if (!roleExists)
            {
                return Result<List<RolePermissionResponseModel>>.NotFound("Role not found.");
            }

            var rolePermissions = await _dbContext.TblRolePermissions
                .Include(x => x.Role)
                .Include(x => x.Permission)
                .Where(x => x.RoleId == roleId && !x.DeleteFlag)
                .ToListAsync();
            var resModel = MapRolePermissions(rolePermissions);

            string message = rolePermissions.Count == 0
                ? "No permissions assigned to this role."
                : "Success";
            return Result<List<RolePermissionResponseModel>>.Success(resModel, message);
        }
        catch (Exception ex)
        {
            return Result<List<RolePermissionResponseModel>>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Assign Permission To Role
    public async Task<Result<RolePermissionResponseModel>> AssignPermissionToRoleAsync(
        AssignPermissionToRoleRequestModel request)
    {
        try
        {
            if (!request.RoleId.IsPositive())
            {
                return Result<RolePermissionResponseModel>.ValidationError("Role ID must be greater than zero.");
            }

            if (!request.PermissionId.IsPositive())
            {
                return Result<RolePermissionResponseModel>.ValidationError("Permission ID must be greater than zero.");
            }

            if (request.AssignedBy.HasValue && !request.AssignedBy.Value.IsPositive())
            {
                return Result<RolePermissionResponseModel>.ValidationError("Assigned by user ID must be greater than zero.");
            }

            var role = await _dbContext.TblRoles
                .FirstOrDefaultAsync(x => x.Id == request.RoleId && x.IsActive && !x.DeleteFlag);
            if (role == null)
            {
                return Result<RolePermissionResponseModel>.ValidationError("Role not found.");
            }

            var permission = await _dbContext.TblPermissions
                .FirstOrDefaultAsync(x => x.Id == request.PermissionId && x.IsActive && !x.DeleteFlag);
            if (permission == null)
            {
                return Result<RolePermissionResponseModel>.ValidationError("Permission not found.");
            }

            var assignedByExists = await _dbContext.TblUsers
                .AnyAsync(x => x.Id == request.AssignedBy && x.IsActive && !x.DeleteFlag);
            if (request.AssignedBy.HasValue && !assignedByExists)
            {
                return Result<RolePermissionResponseModel>.ValidationError("Assigned by user not found.");
            }

            var rolePermission = await _dbContext.TblRolePermissions
                .FirstOrDefaultAsync(x => x.RoleId == request.RoleId
                    && x.PermissionId == request.PermissionId);
            if (rolePermission != null && !rolePermission.DeleteFlag)
            {
                return Result<RolePermissionResponseModel>.ValidationError("Permission is already assigned to this role.");
            }

            if (rolePermission == null)
            {
                rolePermission = new TblRolePermission
                {
                    RoleId = request.RoleId,
                    PermissionId = request.PermissionId
                };
                _dbContext.TblRolePermissions.Add(rolePermission);
            }

            rolePermission.AssignedAt = DateTime.UtcNow;
            rolePermission.AssignedBy = request.AssignedBy;
            rolePermission.DeleteFlag = false;
            await _dbContext.SaveChangesAsync();

            rolePermission.Role = role;
            rolePermission.Permission = permission;
            var resModel = MapRolePermission(rolePermission);
            return Result<RolePermissionResponseModel>.Success(
                resModel,
                "Permission assigned to role successfully.");
        }
        catch (Exception ex)
        {
            return Result<RolePermissionResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Remove Permission From Role
    public async Task<Result<RolePermissionResponseModel>> RemovePermissionFromRoleAsync(
        RemovePermissionFromRoleRequestModel request)
    {
        try
        {
            if (!request.RoleId.IsPositive())
            {
                return Result<RolePermissionResponseModel>.ValidationError("Role ID must be greater than zero.");
            }

            if (!request.PermissionId.IsPositive())
            {
                return Result<RolePermissionResponseModel>.ValidationError("Permission ID must be greater than zero.");
            }

            var rolePermission = await _dbContext.TblRolePermissions
                .FirstOrDefaultAsync(x => x.RoleId == request.RoleId
                    && x.PermissionId == request.PermissionId
                    && !x.DeleteFlag);
            if (rolePermission == null)
            {
                return Result<RolePermissionResponseModel>.NotFound("Role permission assignment not found.");
            }

            rolePermission.DeleteFlag = true;
            await _dbContext.SaveChangesAsync();
            return Result<RolePermissionResponseModel>.DeleteSuccess(
                "Permission removed from role successfully.");
        }
        catch (Exception ex)
        {
            return Result<RolePermissionResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion

    private static List<RolePermissionResponseModel> MapRolePermissions(
        IEnumerable<TblRolePermission> rolePermissions)
    {
        return rolePermissions.Select(MapRolePermission).ToList();
    }

    private static RolePermissionResponseModel MapRolePermission(TblRolePermission rolePermission)
    {
        return new RolePermissionResponseModel
        {
            Id = rolePermission.Id,
            RoleId = rolePermission.RoleId,
            RoleName = rolePermission.Role.Name,
            PermissionId = rolePermission.PermissionId,
            PermissionName = rolePermission.Permission.Name,
            PermissionCode = rolePermission.Permission.Code,
            AssignedAt = rolePermission.AssignedAt,
            AssignedBy = rolePermission.AssignedBy
        };
    }
}
