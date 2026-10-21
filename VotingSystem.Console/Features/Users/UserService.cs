using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VotingSystem.Console.MemoryData;
using VotingSystem.Console.Models;
namespace VotingSystem.Console.Features.Users;

public class UserService : IUserService
{
    public User CreateUser(string username, string password, Enums.UserRoles role)
    {
        int id = Data.MaxUserId + 1;
        User model = new User
        {
            Id = id,
            Username = username,
            Password = password,
            Role = role
        };
        Data.UsersData.Add(model);
        return model;
    }

    public bool DeleteUser(string username)
    {
        var user = Data.UsersData.FirstOrDefault(x => x.Username == username);
        if (user is not null)
        {
            Data.UsersData.Remove(user);
            return true;
        }
        return false;

    }

    public User? UpdateUser(string username, string newPassword, Enums.UserRoles newRole)
    {
        var user = Data.UsersData.FirstOrDefault(x => x.Username == username);
        if (user is not null)
        {
            user.Password = newPassword;
            user.Role = newRole;
            return user;
        }
        return null;
    }

    public User? GetUser(string username)
    {
        if (Data.UsersData is null || Data.UsersData.Count == 0)
        {
            return null;
        }
        return Data.UsersData.FirstOrDefault(x => x.Username == username);
    }

    public List<User>? GetUsers()
    {
        if (Data.UsersData is null || Data.UsersData.Count == 0)
        {
            return null;
        }
        return Data.UsersData;
    }


}
