using System;
using System.Collections.Generic;

namespace VotingSystem.DataAccess.AppDbContextModels;

public partial class TblEventAdmin
{
    public int EventId { get; set; }

    public int AdminId { get; set; }

    public bool? DeleteFlag { get; set; }

    public DateTime? CreateTime { get; set; }

    public int? CreateBy { get; set; }

    public DateTime? ModifyTime { get; set; }

    public int? ModifyBy { get; set; }

    public virtual TblUser Admin { get; set; } = null!;

    public virtual TblEvent Event { get; set; } = null!;
}
