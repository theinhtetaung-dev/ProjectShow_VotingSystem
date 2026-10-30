using System;
using System.Collections.Generic;

namespace VotingSystem.DataAccess.AppDbContextModels;

public partial class TblProjectType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool DeleteFlag { get; set; }

    public virtual ICollection<TblProject> TblProjects { get; set; } = new List<TblProject>();

    public virtual ICollection<TblVotingLimit> TblVotingLimits { get; set; } = new List<TblVotingLimit>();
}
