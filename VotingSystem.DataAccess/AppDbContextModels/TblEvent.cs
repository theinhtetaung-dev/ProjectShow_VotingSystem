using System;
using System.Collections.Generic;

namespace VotingSystem.DataAccess.AppDbContextModels;

public partial class TblEvent
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime EventStartAt { get; set; }

    public DateTime EventEndAt { get; set; }

    public DateTime VotingStartAt { get; set; }

    public DateTime VotingEndAt { get; set; }

    public DateTime? ResultShowStartAt { get; set; }

    public DateTime? ResultShowEndAt { get; set; }

    public string Status { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool DeleteFlag { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public long? ModifiedBy { get; set; }

    public virtual ICollection<TblAdminEvent> TblAdminEvents { get; set; } = new List<TblAdminEvent>();

    public virtual ICollection<TblProject> TblProjects { get; set; } = new List<TblProject>();

    public virtual ICollection<TblResult> TblResults { get; set; } = new List<TblResult>();

    public virtual ICollection<TblUserEvent> TblUserEvents { get; set; } = new List<TblUserEvent>();
}
