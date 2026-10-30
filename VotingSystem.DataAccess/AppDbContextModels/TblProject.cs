using System;
using System.Collections.Generic;

namespace VotingSystem.DataAccess.AppDbContextModels;

public partial class TblProject
{
    public long Id { get; set; }

    public long EventId { get; set; }

    public int ProjectTypeId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int VoteCounts { get; set; }

    public bool IsActive { get; set; }

    public bool DeleteFlag { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public long? ModifiedBy { get; set; }

    public virtual TblUser? CreatedByNavigation { get; set; }

    public virtual TblEvent Event { get; set; } = null!;

    public virtual TblUser? ModifiedByNavigation { get; set; }

    public virtual TblProjectType ProjectType { get; set; } = null!;

    public virtual ICollection<TblResult> TblResults { get; set; } = new List<TblResult>();

    public virtual ICollection<TblVote> TblVotes { get; set; } = new List<TblVote>();
}
