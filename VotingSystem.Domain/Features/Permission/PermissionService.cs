using System.Data;
using Microsoft.EntityFrameworkCore;
using VotingSystem.DataAccess.AppDbContextModels;
using VotingSystem.Shared;

namespace VotingSystem.Domain.Features.Permission;

public class PermissionService : IPermissionService
{
    private readonly AppDbContext _dbContext;

    public PermissionService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region Get Permission List
    public async Task<Result<List<PermissionResponseModel>>> GetPermissionlist()
    {
        try
        {
            var permissions = await _dbContext.TblPermissions
                .Where(x => !x.DeleteFlag)
                .ToListAsync();
            var resModel = new List<PermissionResponseModel>();

            foreach (var permission in permissions)
            {
                resModel.Add(new PermissionResponseModel
                {
                    Id = permission.Id,
                    Name = permission.Name,
                    Code = permission.Code,
                    Description = permission.Description
                });
            }

            string message = permissions.Count == 0 ? "No permissions found." : "Success";
            return Result<List<PermissionResponseModel>>.Success(resModel, message);
        }
        catch (Exception ex)
        {
            return Result<List<PermissionResponseModel>>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Get Permission By ID
    public async Task<Result<PermissionResponseModel>> GetPermissionByID(int id)
    {
        try
        {
            var permission = await _dbContext.TblPermissions
                .FirstOrDefaultAsync(x => x.Id == id && !x.DeleteFlag);
            if (permission == null)
            {
                return Result<PermissionResponseModel>.NotFound("Item Not Found!");
            }

            var resModel = new PermissionResponseModel
            {
                Id = permission.Id,
                Name = permission.Name,
                Code = permission.Code,
                Description = permission.Description
            };
            return Result<PermissionResponseModel>.Success(resModel, "Success");
        }
        catch (Exception ex)
        {
            return Result<PermissionResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Create Permission
    public async Task<Result<PermissionResponseModel>> CreatePermissionAsync(PermissionCreateRequestModel request)
    {
        try
        {
            if (!request.Name.IsValidPermissionName())
            {
                return Result<PermissionResponseModel>.ValidationError("Permission name must use Resource.Action format and must not exceed 100 characters.");
            }

            if (!request.Description.IsValidOptionalString(255))
            {
                return Result<PermissionResponseModel>.ValidationError("Description must not exceed 255 characters.");
            }

            await using var transaction = await _dbContext.Database
                .BeginTransactionAsync(IsolationLevel.Serializable);

            var existingCodes = await _dbContext.TblPermissions
                .Where(x => x.Code.StartsWith("Per-"))
                .Select(x => x.Code)
                .ToListAsync();

            var nextNumber = existingCodes
                .Select(code => int.TryParse(code[4..], out var number) ? number : 0)
                .DefaultIfEmpty()
                .Max() + 1;
            var permissionCode = $"Per-{nextNumber:D4}";

            var newPermission = new TblPermission
            {
                Name = request.Name,
                Code = permissionCode,
                Description = request.Description,
                IsActive = true,
                DeleteFlag = false,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.TblPermissions.Add(newPermission);
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            var resModel = new PermissionResponseModel
            {
                Id = newPermission.Id,
                Name = newPermission.Name,
                Code = newPermission.Code,
                Description = newPermission.Description
            };
            return Result<PermissionResponseModel>.Success(resModel, "Permission created successfully.");
        }
        catch (Exception ex)
        {
            return Result<PermissionResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Update Permission
    public async Task<Result<PermissionResponseModel>> UpdatePermission(PermissionUpdateRequestModel request)
    {
        try
        {
            var permission = await _dbContext.TblPermissions
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.DeleteFlag);
            if (permission == null)
            {
                return Result<PermissionResponseModel>.NotFound("Item Not Found!");
            }

            if (request.Name != null && !request.Name.IsValidPermissionName())
            {
                return Result<PermissionResponseModel>.ValidationError("Permission name must use Resource.Action format and must not exceed 100 characters.");
            }

            if (request.Description != null && !request.Description.IsValidOptionalString(255))
            {
                return Result<PermissionResponseModel>.ValidationError("Description must not exceed 255 characters.");
            }

            if (request.Name != null)
            {
                permission.Name = request.Name;
            }

            if (request.Description != null)
            {
                permission.Description = request.Description;
            }

            var hasUpdates = request.Name != null
                || request.Description != null;
            if (hasUpdates)
            {
                permission.ModifiedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }

            var resModel = new PermissionResponseModel
            {
                Id = permission.Id,
                Name = permission.Name,
                Code = permission.Code,
                Description = permission.Description
            };
            return Result<PermissionResponseModel>.Success(resModel, "Permission updated successfully.");
        }
        catch (Exception ex)
        {
            return Result<PermissionResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Delete Permission
    public async Task<Result<PermissionResponseModel>> DeletePermission(int id)
    {
        try
        {
            var permission = await _dbContext.TblPermissions
                .FirstOrDefaultAsync(x => x.Id == id && !x.DeleteFlag);
            if (permission == null)
            {
                return Result<PermissionResponseModel>.NotFound("Item Not Found!");
            }

            permission.DeleteFlag = true;
            permission.IsActive = false;
            permission.ModifiedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return Result<PermissionResponseModel>.DeleteSuccess("Permission deleted successfully.");
        }
        catch (Exception ex)
        {
            return Result<PermissionResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion
}
