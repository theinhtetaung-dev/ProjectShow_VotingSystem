using VotingSystem.Console.MemoryData;

namespace VotingSystem.Console.Features
{
    public interface IAuthService
    {
        bool Login(string username, string password);
        bool Register(string username, string password, Enums.UserRoles role);
    }
}