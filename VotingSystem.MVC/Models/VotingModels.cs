namespace VotingSystem.MVC.Models;

public class VoteRequestModel { public long UserEventId { get; set; } public long ProjectId { get; set; } }
public class VotingLimitUpdateRequestModel { public long? EventId { get; set; } public long UserEventId { get; set; } public int ProjectTypeId { get; set; } public int VoteLimit { get; set; } }
public class VotingResponseModel { public long Id { get; set; } public long UserEventId { get; set; } public long ProjectId { get; set; } public DateTime VotedAt { get; set; } }
