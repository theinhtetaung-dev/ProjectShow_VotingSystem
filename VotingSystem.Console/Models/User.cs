using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VotingSystem.Console.MemoryData;

namespace VotingSystem.Console.Models;

public class User
{
    public int Id { get; set; }

    public Enums.UserRoles Role { get; set; } = Enums.UserRoles.User;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public int votingCount { get; set; } = 1;
}
