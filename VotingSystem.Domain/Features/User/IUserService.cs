using VotingSystem.Shared;

namespace VotingSystem.Domain.Features.User;

public interface IUserService
{
    Task<Result<UserResponseModel>> CreateUserAsync(UserCreateRequestModel request);
    Task<Result<UserResponseModel>> DeleteUser(long id);
    Task<Result<UserResponseModel>> GetUserByID(long id);
    Task<Result<List<UserResponseModel>>> GetUserlist();
    Task<Result<UserResponseModel>> UpdateUser(UserUpdateRequestModel request);
}
