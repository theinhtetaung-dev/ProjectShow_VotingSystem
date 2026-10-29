using Microsoft.EntityFrameworkCore;
using VotingSystem.DataAccess.AppDbContextModels;
using VotingSystem.Shared;

namespace VotingSystem.Domain.Features.Role;

public class RoleService : IRoleService
{
    private readonly AppDbContext _dbContext;

    public RoleService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region Get Role List
    public async Task<Result<List<RoleResponseModel>>> GetRolelist()
    {
        try
        {
            var roles = await _dbContext.TblRoles
                .Where(x => !x.DeleteFlag)
                .ToListAsync();
            var resModel = new List<RoleResponseModel>();

            foreach (var role in roles)
            {
                resModel.Add(new RoleResponseModel
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description
                });
            }

            string message = roles.Count == 0 ? "No roles found." : "Success";
            return Result<List<RoleResponseModel>>.Success(resModel, message);
        }
        catch (Exception ex)
        {
            return Result<List<RoleResponseModel>>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Get Role By ID
    public async Task<Result<RoleResponseModel>> GetRoleByID(int id)
    {
        try
        {
            var role = await _dbContext.TblRoles
                .FirstOrDefaultAsync(x => x.Id == id && !x.DeleteFlag);
            if (role == null)
            {
                return Result<RoleResponseModel>.NotFound("Item Not Found!");
            }

            var resModel = new RoleResponseModel
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description
            };
            return Result<RoleResponseModel>.Success(resModel, "Success");
        }
        catch (Exception ex)
        {
            return Result<RoleResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Create Role
    public async Task<Result<RoleResponseModel>> CreateRoleAsync(RoleCreateRequestModel request)
    {
        try
        {
            if (!request.Name.IsValidRequiredString(50))
            {
                return Result<RoleResponseModel>.ValidationError("Name is required and must not exceed 50 characters.");
            }

            if (!request.Description.IsValidOptionalString(255))
            {
                return Result<RoleResponseModel>.ValidationError("Description must not exceed 255 characters.");
            }

            var nameExists = await _dbContext.TblRoles
                .AnyAsync(x => x.Name == request.Name);
            if (nameExists)
            {
                return Result<RoleResponseModel>.ValidationError("Role name already exists.");
            }

            var newRole = new TblRole
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = true,
                DeleteFlag = false,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.TblRoles.Add(newRole);
            await _dbContext.SaveChangesAsync();

            var resModel = new RoleResponseModel
            {
                Id = newRole.Id,
                Name = newRole.Name,
                Description = newRole.Description
            };
            return Result<RoleResponseModel>.Success(resModel, "Role created successfully.");
        }
        catch (Exception ex)
        {
            return Result<RoleResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Update Role
    public async Task<Result<RoleResponseModel>> UpdateRole(RoleUpdateRequestModel request)
    {
        try
        {
            var role = await _dbContext.TblRoles
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.DeleteFlag);
            if (role == null)
            {
                return Result<RoleResponseModel>.NotFound("Item Not Found!");
            }

            if (request.Name != null && !request.Name.IsValidRequiredString(50))
            {
                return Result<RoleResponseModel>.ValidationError("Name is required and must not exceed 50 characters.");
            }

            if (request.Description != null && !request.Description.IsValidOptionalString(255))
            {
                return Result<RoleResponseModel>.ValidationError("Description must not exceed 255 characters.");
            }

            var nameExists = await _dbContext.TblRoles
                .AnyAsync(x => x.Name == request.Name && x.Id != request.Id);
            if (request.Name != null && nameExists)
            {
                return Result<RoleResponseModel>.ValidationError("Role name already exists.");
            }

            if (request.Name != null)
            {
                role.Name = request.Name;
            }

            if (request.Description != null)
            {
                role.Description = request.Description;
            }

            var hasUpdates = request.Name != null || request.Description != null;
            if (hasUpdates)
            {
                role.ModifiedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }

            var resModel = new RoleResponseModel
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description
            };
            return Result<RoleResponseModel>.Success(resModel, "Role updated successfully.");
        }
        catch (Exception ex)
        {
            return Result<RoleResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Delete Role
    public async Task<Result<RoleResponseModel>> DeleteRole(int id)
    {
        try
        {
            var role = await _dbContext.TblRoles
                .FirstOrDefaultAsync(x => x.Id == id && !x.DeleteFlag);
            if (role == null)
            {
                return Result<RoleResponseModel>.NotFound("Item Not Found!");
            }

            role.DeleteFlag = true;
            role.IsActive = false;
            role.ModifiedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return Result<RoleResponseModel>.DeleteSuccess("Role deleted successfully.");
        }
        catch (Exception ex)
        {
            return Result<RoleResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion
}
