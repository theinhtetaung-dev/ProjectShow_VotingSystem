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

            var emailExists = await _dbContext.TblUsers
                .AnyAsync(x => x.Email == request.Email);
            if (emailExists)
            {
                return Result<UserResponseModel>.ValidationError("Email already exists.");
            }

            var roleExists = await _dbContext.TblRoles
                .AnyAsync(x => x.Id == request.RoleId && !x.DeleteFlag);
            if (!roleExists)
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

            var emailExists = await _dbContext.TblUsers
                .AnyAsync(x => x.Email == request.Email && x.Id != request.Id);
            if (request.Email != null && emailExists)
            {
                return Result<UserResponseModel>.ValidationError("Email already exists.");
            }

            var roleExists = await _dbContext.TblRoles
                .AnyAsync(x => x.Id == request.RoleId && !x.DeleteFlag);
            if (request.RoleId.HasValue && !roleExists)
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

    #region Assign User To Event
    public async Task<Result<UserEventResponseModel>> AssignUserToEventAsync(AssignUserToEventRequestModel request)
    {
        try
        {
            if (!request.UserId.IsPositive())
            {
                return Result<UserEventResponseModel>.ValidationError("User ID must be greater than zero.");
            }

            if (!request.EventId.IsPositive())
            {
                return Result<UserEventResponseModel>.ValidationError("Event ID must be greater than zero.");
            }

            if (!request.EventPasswordHash.IsValidRequiredString(500))
            {
                return Result<UserEventResponseModel>.ValidationError("Event password hash is required and must not exceed 500 characters.");
            }

            var currentTime = DateTime.UtcNow;
            if (request.PasswordExpiresAt <= currentTime)
            {
                return Result<UserEventResponseModel>.ValidationError("Password expiry date must be later than the current date.");
            }

            var user = await _dbContext.TblUsers
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == request.UserId && x.IsActive && !x.DeleteFlag);
            if (user == null)
            {
                return Result<UserEventResponseModel>.ValidationError("User not found.");
            }

            if (user.Role.Name != "User")
            {
                return Result<UserEventResponseModel>.ValidationError("Only users with the User role can be assigned as event users.");
            }

            var eventExists = await _dbContext.TblEvents
                .AnyAsync(x => x.Id == request.EventId && x.IsActive && !x.DeleteFlag);
            if (!eventExists)
            {
                return Result<UserEventResponseModel>.ValidationError("Event not found.");
            }

            var assignedByExists = await _dbContext.TblUsers
                .AnyAsync(x => x.Id == request.AssignedBy && x.IsActive && !x.DeleteFlag);
            if (request.AssignedBy.HasValue && !assignedByExists)
            {
                return Result<UserEventResponseModel>.ValidationError("Assigned by user not found.");
            }

            var userEvent = await _dbContext.TblUserEvents
                .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.EventId == request.EventId);
            if (userEvent != null && !userEvent.DeleteFlag)
            {
                return Result<UserEventResponseModel>.ValidationError("User is already assigned to this event.");
            }

            if (userEvent == null)
            {
                userEvent = new TblUserEvent
                {
                    UserId = request.UserId,
                    EventId = request.EventId
                };
                _dbContext.TblUserEvents.Add(userEvent);
            }

            userEvent.EventPasswordHash = request.EventPasswordHash;
            userEvent.AssignedAt = currentTime;
            userEvent.AssignedBy = request.AssignedBy;
            userEvent.PasswordGeneratedAt = currentTime;
            userEvent.PasswordExpiresAt = request.PasswordExpiresAt;
            userEvent.AccessStatus = "valid";
            userEvent.RemovedAt = null;
            userEvent.RemovedBy = null;
            userEvent.IsActive = true;
            userEvent.DeleteFlag = false;
            await _dbContext.SaveChangesAsync();

            var resModel = new UserEventResponseModel
            {
                Id = userEvent.Id,
                UserId = userEvent.UserId,
                EventId = userEvent.EventId,
                AssignedAt = userEvent.AssignedAt,
                AssignedBy = userEvent.AssignedBy,
                PasswordGeneratedAt = userEvent.PasswordGeneratedAt,
                PasswordExpiresAt = userEvent.PasswordExpiresAt,
                AccessStatus = userEvent.AccessStatus
            };
            return Result<UserEventResponseModel>.Success(resModel, "User assigned to event successfully.");
        }
        catch (Exception ex)
        {
            return Result<UserEventResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Remove User From Event
    public async Task<Result<UserEventResponseModel>> RemoveUserFromEventAsync(RemoveUserFromEventRequestModel request)
    {
        try
        {
            if (!request.UserId.IsPositive())
            {
                return Result<UserEventResponseModel>.ValidationError("User ID must be greater than zero.");
            }

            if (!request.EventId.IsPositive())
            {
                return Result<UserEventResponseModel>.ValidationError("Event ID must be greater than zero.");
            }

            var removedByExists = await _dbContext.TblUsers
                .AnyAsync(x => x.Id == request.RemovedBy && x.IsActive && !x.DeleteFlag);
            if (request.RemovedBy.HasValue && !removedByExists)
            {
                return Result<UserEventResponseModel>.ValidationError("Removed by user not found.");
            }

            var userEvent = await _dbContext.TblUserEvents
                .FirstOrDefaultAsync(x => x.UserId == request.UserId
                    && x.EventId == request.EventId
                    && !x.DeleteFlag);
            if (userEvent == null)
            {
                return Result<UserEventResponseModel>.NotFound("User event assignment not found.");
            }

            userEvent.AccessStatus = "expired";
            userEvent.RemovedAt = DateTime.UtcNow;
            userEvent.RemovedBy = request.RemovedBy;
            userEvent.IsActive = false;
            userEvent.DeleteFlag = true;
            await _dbContext.SaveChangesAsync();

            return Result<UserEventResponseModel>.DeleteSuccess("User removed from event successfully.");
        }
        catch (Exception ex)
        {
            return Result<UserEventResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Assign Admin To Event
    public async Task<Result<AdminEventResponseModel>> AssignAdminToEventAsync(AssignAdminToEventRequestModel request)
    {
        try
        {
            if (!request.AdminUserId.IsPositive())
            {
                return Result<AdminEventResponseModel>.ValidationError("Admin user ID must be greater than zero.");
            }

            if (!request.EventId.IsPositive())
            {
                return Result<AdminEventResponseModel>.ValidationError("Event ID must be greater than zero.");
            }

            var adminUser = await _dbContext.TblUsers
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == request.AdminUserId && x.IsActive && !x.DeleteFlag);
            if (adminUser == null)
            {
                return Result<AdminEventResponseModel>.ValidationError("Admin user not found.");
            }

            var isAdmin = adminUser.Role.Name == "Admin" || adminUser.Role.Name == "SuperAdmin";
            if (!isAdmin)
            {
                return Result<AdminEventResponseModel>.ValidationError("Only users with an Admin or SuperAdmin role can be assigned as event admins.");
            }

            var eventExists = await _dbContext.TblEvents
                .AnyAsync(x => x.Id == request.EventId && x.IsActive && !x.DeleteFlag);
            if (!eventExists)
            {
                return Result<AdminEventResponseModel>.ValidationError("Event not found.");
            }

            var assignedByExists = await _dbContext.TblUsers
                .AnyAsync(x => x.Id == request.AssignedBy && x.IsActive && !x.DeleteFlag);
            if (request.AssignedBy.HasValue && !assignedByExists)
            {
                return Result<AdminEventResponseModel>.ValidationError("Assigned by user not found.");
            }

            var adminEvent = await _dbContext.TblAdminEvents
                .FirstOrDefaultAsync(x => x.AdminUserId == request.AdminUserId && x.EventId == request.EventId);
            if (adminEvent != null && !adminEvent.DeleteFlag)
            {
                return Result<AdminEventResponseModel>.ValidationError("Admin is already assigned to this event.");
            }

            if (adminEvent == null)
            {
                adminEvent = new TblAdminEvent
                {
                    AdminUserId = request.AdminUserId,
                    EventId = request.EventId
                };
                _dbContext.TblAdminEvents.Add(adminEvent);
            }

            adminEvent.AssignedAt = DateTime.UtcNow;
            adminEvent.AssignedBy = request.AssignedBy;
            adminEvent.IsActive = true;
            adminEvent.DeleteFlag = false;
            await _dbContext.SaveChangesAsync();

            var resModel = new AdminEventResponseModel
            {
                Id = adminEvent.Id,
                AdminUserId = adminEvent.AdminUserId,
                EventId = adminEvent.EventId,
                AssignedAt = adminEvent.AssignedAt,
                AssignedBy = adminEvent.AssignedBy
            };
            return Result<AdminEventResponseModel>.Success(resModel, "Admin assigned to event successfully.");
        }
        catch (Exception ex)
        {
            return Result<AdminEventResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Remove Admin From Event
    public async Task<Result<AdminEventResponseModel>> RemoveAdminFromEventAsync(RemoveAdminFromEventRequestModel request)
    {
        try
        {
            if (!request.AdminUserId.IsPositive())
            {
                return Result<AdminEventResponseModel>.ValidationError("Admin user ID must be greater than zero.");
            }

            if (!request.EventId.IsPositive())
            {
                return Result<AdminEventResponseModel>.ValidationError("Event ID must be greater than zero.");
            }

            var adminEvent = await _dbContext.TblAdminEvents
                .FirstOrDefaultAsync(x => x.AdminUserId == request.AdminUserId
                    && x.EventId == request.EventId
                    && !x.DeleteFlag);
            if (adminEvent == null)
            {
                return Result<AdminEventResponseModel>.NotFound("Admin event assignment not found.");
            }

            adminEvent.IsActive = false;
            adminEvent.DeleteFlag = true;
            await _dbContext.SaveChangesAsync();

            return Result<AdminEventResponseModel>.DeleteSuccess("Admin removed from event successfully.");
        }
        catch (Exception ex)
        {
            return Result<AdminEventResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion
}
