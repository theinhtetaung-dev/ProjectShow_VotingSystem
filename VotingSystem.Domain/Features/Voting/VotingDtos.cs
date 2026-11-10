namespace VotingSystem.Domain.Features.Voting;

public class VotingLimitUpdateRequestModel
{
    public long? EventId { get; set; }
    public long UserEventId { get; set; }
    public int ProjectTypeId { get; set; }
    public int VoteLimit { get; set; }
}

public class VoteRequestModel
{
    public long UserEventId { get; set; }
    public long ProjectId { get; set; }
}

public class VotingLimitResponseModel
{
    public long Id { get; set; }
    public long UserEventId { get; set; }
    public int ProjectTypeId { get; set; }
    public int VoteLimit { get; set; }
    public int VoteUsed { get; set; }
}

public class VotingResponseModel
{
    public long Id { get; set; }
    public long UserEventId { get; set; }
    public long ProjectId { get; set; }
    public DateTime VotedAt { get; set; }
}

public class VoteResponseModel : VotingResponseModel
{
    public int VoteLimit { get; set; }
    public int VoteUsed { get; set; }
    public int ProjectVoteCounts { get; set; }
}
