using System;
using System.Collections.Generic;

namespace VotingSystem.DataAccess.AppDbContextModels;

public partial class TblVote
{
    public long Id { get; set; }

    public long UserEventId { get; set; }

    public long ProjectId { get; set; }

    public DateTime VotedAt { get; set; }

    public virtual TblProject Project { get; set; } = null!;

    public virtual TblUserEvent UserEvent { get; set; } = null!;
}
