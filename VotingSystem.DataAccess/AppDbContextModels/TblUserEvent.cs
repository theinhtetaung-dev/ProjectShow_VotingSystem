using System;
using System.Collections.Generic;

namespace VotingSystem.DataAccess.AppDbContextModels;

public partial class TblUserEvent
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long EventId { get; set; }

    public string EventPasswordHash { get; set; } = null!;

    public DateTime AssignedAt { get; set; }

    public long? AssignedBy { get; set; }

    public DateTime PasswordGeneratedAt { get; set; }

    public DateTime PasswordExpiresAt { get; set; }

    public string AccessStatus { get; set; } = null!;

    public DateTime? RemovedAt { get; set; }

    public long? RemovedBy { get; set; }

    public bool IsActive { get; set; }

    public bool DeleteFlag { get; set; }

    public virtual TblUser? AssignedByNavigation { get; set; }

    public virtual TblEvent Event { get; set; } = null!;

    public virtual TblUser? RemovedByNavigation { get; set; }

    public virtual ICollection<TblVote> TblVotes { get; set; } = new List<TblVote>();

    public virtual ICollection<TblVotingLimit> TblVotingLimits { get; set; } = new List<TblVotingLimit>();

    public virtual TblUser User { get; set; } = null!;
}
