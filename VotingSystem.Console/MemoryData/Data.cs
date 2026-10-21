using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VotingSystem.Console.Models;

namespace VotingSystem.Console.MemoryData;

public static class Data
{
    public static List<User> UsersData = new List<User>
    {
        new User { Id = 1, Username = "admin", Password = "admin123", Role = Enums.UserRoles.Admin, votingCount = 0 },
        new User { Id = 2, Username = "user1", Password = "user123", Role = Enums.UserRoles.User, votingCount = 1 },
        new User { Id = 3, Username = "User2", Password = "user456", Role = Enums.UserRoles.User, votingCount = 1 }
    };

    public static List<Project> ProjectsData = new List<Project>
    {
        new Project { Id = 1, Name = "Canteen Food Order System", Description = "Description for Project A", VoteCount = 0 },
        new Project { Id = 2, Name = "Sar Mal", Description = "Description for Project B", VoteCount = 0 },
        new Project { Id = 3, Name = "Robotic Hand", Description = "Description for Project C", VoteCount = 0 }
    };

    public static int MaxUserId = UsersData.MaxBy(x => x.Id) ? .Id ?? 0;
    public static int MaxProjId = ProjectsData.MaxBy(x => x.Id)?.Id ?? 0;
}
