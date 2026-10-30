using System;
using System.Collections.Generic;

namespace VotingSystem.DataAccess.AppDbContextModels;

public partial class TblVotingLimit
{
    public long Id { get; set; }

    public long UserEventId { get; set; }

    public int ProjectTypeId { get; set; }

    public int VoteLimit { get; set; }

    public int VoteUsed { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public long? ModifiedBy { get; set; }

    public bool DeleteFlag { get; set; }

    public virtual TblUser? CreatedByNavigation { get; set; }

    public virtual TblUser? ModifiedByNavigation { get; set; }

    public virtual TblProjectType ProjectType { get; set; } = null!;

    public virtual TblUserEvent UserEvent { get; set; } = null!;
}
