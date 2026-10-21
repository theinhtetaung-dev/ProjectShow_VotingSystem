namespace VotingSystem.Console.Features.Votes
{
    public interface IVotingService
    {
        int UpdateUserVoteCount(string Username, int newVoteCount);
        bool VoteProject(int ProjectID, string Username);
    }
}