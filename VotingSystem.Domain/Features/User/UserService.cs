using Microsoft.EntityFrameworkCore;
using VotingSystem.DataAccess.AppDbContextModels;
using VotingSystem.Shared;

namespace VotingSystem.Domain.Features.User;

public class UserService : IUserService
{
    private readonly AppDbContext _dbContext;

    public UserService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region Get User List
    public async Task<Result<List<UserResponseModel>>> GetUserlist()
    {
        try
        {
            var users = await _dbContext.TblUsers
                .Where(x => !x.DeleteFlag)
                .ToListAsync();
            var resModel = new List<UserResponseModel>();

            foreach (var user in users)
            {
                resModel.Add(new UserResponseModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    RoleId = user.RoleId
                });
            }

            string message = users.Count == 0 ? "No users found." : "Success";
            return Result<List<UserResponseModel>>.Success(resModel, message);
        }
        catch (Exception ex)
        {
            return Result<List<UserResponseModel>>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Get User By ID
    public async Task<Result<UserResponseModel>> GetUserByID(long id)
    {
        try
        {
            var user = await _dbContext.TblUsers
                .FirstOrDefaultAsync(x => x.Id == id && !x.DeleteFlag);
            if (user == null)
            {
                return Result<UserResponseModel>.NotFound("Item Not Found!");
            }

            var resModel = new UserResponseModel
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                RoleId = user.RoleId
            };
            return Result<UserResponseModel>.Success(resModel, "Success");
        }
        catch (Exception ex)
        {
            return Result<UserResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Create User
    public async Task<Result<UserResponseModel>> CreateUserAsync(UserCreateRequestModel request)
    {
        try
        {
            if (!request.Email.IsValidEmail(255))
            {
                return Result<UserResponseModel>.ValidationError("Email is required, must be valid, and must not exceed 255 characters.");
            }

            if (!request.Name.IsValidRequiredString(150))
            {
                return Result<UserResponseModel>.ValidationError("Name is required and must not exceed 150 characters.");
            }

            if (!request.PasswordHash.IsValidOptionalString(500))
            {
                return Result<UserResponseModel>.ValidationError("Password hash must not exceed 500 characters.");
            }

            if (!request.RoleId.IsPositive())
            {
                return Result<UserResponseModel>.ValidationError("Role ID must be greater than zero.");
            }

            if (await _dbContext.TblUsers.AnyAsync(x => x.Email == request.Email))
            {
                return Result<UserResponseModel>.ValidationError("Email already exists.");
            }

            if (!await _dbContext.TblRoles.AnyAsync(x => x.Id == request.RoleId && !x.DeleteFlag))
            {
                return Result<UserResponseModel>.ValidationError("Role not found.");
            }

            var newUser = new TblUser
            {
                Email = request.Email,
                Name = request.Name,
                PasswordHash = request.PasswordHash,
                RoleId = request.RoleId,
                IsActive = true,
                DeleteFlag = false,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.TblUsers.Add(newUser);
            await _dbContext.SaveChangesAsync();

            var resModel = new UserResponseModel
            {
                Id = newUser.Id,
                Email = newUser.Email,
                Name = newUser.Name,
                RoleId = newUser.RoleId
            };
            return Result<UserResponseModel>.Success(resModel, "User created successfully.");
        }
        catch (Exception ex)
        {
            return Result<UserResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Update User
    public async Task<Result<UserResponseModel>> UpdateUser(UserUpdateRequestModel request)
    {
        try
        {
            var user = await _dbContext.TblUsers
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.DeleteFlag);
            if (user == null)
            {
                return Result<UserResponseModel>.NotFound("Item Not Found!");
            }

            if (request.Email != null && !request.Email.IsValidEmail(255))
            {
                return Result<UserResponseModel>.ValidationError("Email is required, must be valid, and must not exceed 255 characters.");
            }

            if (request.Name != null && !request.Name.IsValidRequiredString(150))
            {
                return Result<UserResponseModel>.ValidationError("Name is required and must not exceed 150 characters.");
            }

            if (request.PasswordHash != null && !request.PasswordHash.IsValidOptionalString(500))
            {
                return Result<UserResponseModel>.ValidationError("Password hash must not exceed 500 characters.");
            }

            if (request.RoleId.HasValue && !request.RoleId.Value.IsPositive())
            {
                return Result<UserResponseModel>.ValidationError("Role ID must be greater than zero.");
            }

            if (request.Email != null
                && await _dbContext.TblUsers.AnyAsync(x => x.Email == request.Email && x.Id != request.Id))
            {
                return Result<UserResponseModel>.ValidationError("Email already exists.");
            }

            if (request.RoleId.HasValue
                && !await _dbContext.TblRoles.AnyAsync(x => x.Id == request.RoleId.Value && !x.DeleteFlag))
            {
                return Result<UserResponseModel>.ValidationError("Role not found.");
            }

            if (request.Email != null)
            {
                user.Email = request.Email;
            }

            if (request.Name != null)
            {
                user.Name = request.Name;
            }

            if (request.PasswordHash != null)
            {
                user.PasswordHash = request.PasswordHash;
            }

            if (request.RoleId.HasValue)
            {
                user.RoleId = request.RoleId.Value;
            }

            var hasUpdates = request.Email != null
                || request.Name != null
                || request.PasswordHash != null
                || request.RoleId.HasValue;
            if (hasUpdates)
            {
                user.ModifiedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }

            var resModel = new UserResponseModel
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                RoleId = user.RoleId
            };
            return Result<UserResponseModel>.Success(resModel, "User updated successfully.");
        }
        catch (Exception ex)
        {
            return Result<UserResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Delete User
    public async Task<Result<UserResponseModel>> DeleteUser(long id)
    {
        try
        {
            var user = await _dbContext.TblUsers
                .FirstOrDefaultAsync(x => x.Id == id && !x.DeleteFlag);
            if (user == null)
            {
                return Result<UserResponseModel>.NotFound("Item Not Found!");
            }

            user.DeleteFlag = true;
            user.IsActive = false;
            user.ModifiedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return Result<UserResponseModel>.DeleteSuccess("User deleted successfully.");
        }
        catch (Exception ex)
        {
            return Result<UserResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion
}
