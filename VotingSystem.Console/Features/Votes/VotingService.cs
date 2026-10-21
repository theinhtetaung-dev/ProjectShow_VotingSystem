using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VotingSystem.Console.MemoryData;

namespace VotingSystem.Console.Features.Votes;

public class VotingService : IVotingService
{
    public bool VoteProject(int ProjectID, string Username)
    {
        var user = Data.UsersData.FirstOrDefault(u => u.Username == Username);
        var project = Data.ProjectsData.FirstOrDefault(p => p.Id == ProjectID);
        if (project != null && user != null && user.votingCount != 0)
        {
            project.VoteCount++;
            user.votingCount--;
            return true;
        }
        return false;

    }

    public int UpdateUserVoteCount(string Username, int newVoteCount)
    {
        var user = Data.UsersData.FirstOrDefault(u => u.Username == Username);
        if (user != null)
        {
            user.votingCount = newVoteCount;
            return user.votingCount;
        }
        return -1;
    }
}
