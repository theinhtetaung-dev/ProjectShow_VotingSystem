using VotingSystem.Shared;

namespace VotingSystem.Domain.Features.User;

public interface IUserService
{
    Task<Result<AdminEventResponseModel>> AssignAdminToEventAsync(AssignAdminToEventRequestModel request);
    Task<Result<UserEventResponseModel>> AssignUserToEventAsync(AssignUserToEventRequestModel request);
    Task<Result<UserResponseModel>> CreateUserAsync(UserCreateRequestModel request);
    Task<Result<UserResponseModel>> DeleteUser(long id);
    Task<Result<UserResponseModel>> GetUserByID(long id);
    Task<Result<List<UserResponseModel>>> GetUserlist();
    Task<Result<AdminEventResponseModel>> RemoveAdminFromEventAsync(RemoveAdminFromEventRequestModel request);
    Task<Result<UserEventResponseModel>> RemoveUserFromEventAsync(RemoveUserFromEventRequestModel request);
    Task<Result<UserResponseModel>> UpdateUser(UserUpdateRequestModel request);
}
