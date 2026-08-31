using System;
using System.Collections.Generic;

namespace VotingSystem.DataAccess.AppDbContextModels;

public partial class TblEvent
{
    public int Id { get; set; }

    public string EventName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public bool? DeleteFlag { get; set; }

    public DateTime? CreateTime { get; set; }

    public int? CreateBy { get; set; }

    public DateTime? ModifyTime { get; set; }

    public int? ModifyBy { get; set; }

    public virtual ICollection<TblEventAdmin> TblEventAdmins { get; set; } = new List<TblEventAdmin>();

    public virtual ICollection<TblEventUser> TblEventUsers { get; set; } = new List<TblEventUser>();

    public virtual ICollection<TblProject> TblProjects { get; set; } = new List<TblProject>();

    public virtual ICollection<TblUserVoteLimit> TblUserVoteLimits { get; set; } = new List<TblUserVoteLimit>();

    public virtual ICollection<TblVote> TblVotes { get; set; } = new List<TblVote>();
}
