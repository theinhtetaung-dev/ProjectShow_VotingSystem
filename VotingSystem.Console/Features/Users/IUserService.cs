using VotingSystem.Console.MemoryData;
using VotingSystem.Console.Models;

namespace VotingSystem.Console.Features.Users
{
    public interface IUserService
    {
        User CreateUser(string username, string password, Enums.UserRoles role);
        bool DeleteUser(string username);
        User? GetUser(string username);
        List<User>? GetUsers();
        User? UpdateUser(string username, string newPassword, Enums.UserRoles newRole);
    }
}