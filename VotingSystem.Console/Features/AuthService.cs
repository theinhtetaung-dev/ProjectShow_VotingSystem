using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VotingSystem.Console.MemoryData;
using VotingSystem.Console.Models;

namespace VotingSystem.Console.Features;

public class AuthService : IAuthService
{
    public bool Login(string username, string password)
    {
        var user = Data.UsersData.FirstOrDefault(u => u.Username == username && u.Password == password);
        if (user == null)
        {
            return false;
        }
        return true;
    }

    public bool Register(string username, string password, Enums.UserRoles role)
    {
        var existingUser = Data.UsersData.FirstOrDefault(u => u.Username == username);
        if (existingUser != null)
        {
            return false;
        }
        int id = Data.MaxUserId + 1;
        User newUser = new User
        {
            Id = id,
            Username = username,
            Password = password,
            Role = role
        };
        Data.UsersData.Add(newUser);
        return true;
    }
}
