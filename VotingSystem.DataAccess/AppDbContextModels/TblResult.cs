using System;
using System.Collections.Generic;

namespace VotingSystem.DataAccess.AppDbContextModels;

public partial class TblResult
{
    public long Id { get; set; }

    public long EventId { get; set; }

    public long ProjectId { get; set; }

    public string Status { get; set; } = null!;

    public int Version { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public long? ReviewedBy { get; set; }

    public DateTime? PublishedAt { get; set; }

    public long? PublishedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public long? ModifiedBy { get; set; }

    public bool DeleteFlag { get; set; }

    public virtual TblUser? CreatedByNavigation { get; set; }

    public virtual TblEvent Event { get; set; } = null!;

    public virtual TblUser? ModifiedByNavigation { get; set; }

    public virtual TblProject Project { get; set; } = null!;

    public virtual TblUser? PublishedByNavigation { get; set; }

    public virtual TblUser? ReviewedByNavigation { get; set; }
}
