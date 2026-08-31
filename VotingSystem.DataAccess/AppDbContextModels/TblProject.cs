using System;
using System.Collections.Generic;

namespace VotingSystem.DataAccess.AppDbContextModels;

public partial class TblProject
{
    public int Id { get; set; }

    public int EventId { get; set; }

    public int ProjectTypeId { get; set; }

    public string ProjectName { get; set; } = null!;

    public string? Description { get; set; }

    public bool? DeleteFlag { get; set; }

    public DateTime? CreateTime { get; set; }

    public int? CreateBy { get; set; }

    public DateTime? ModifyTime { get; set; }

    public int? ModifyBy { get; set; }

    public virtual TblEvent Event { get; set; } = null!;

    public virtual TblProjectType ProjectType { get; set; } = null!;

    public virtual ICollection<TblVote> TblVotes { get; set; } = new List<TblVote>();
}
