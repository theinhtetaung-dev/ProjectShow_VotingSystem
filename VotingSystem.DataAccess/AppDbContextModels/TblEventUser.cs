using System;
using System.Collections.Generic;

namespace VotingSystem.DataAccess.AppDbContextModels;

public partial class TblEventUser
{
    public int EventId { get; set; }

    public int UserId { get; set; }

    public bool? DeleteFlag { get; set; }

    public DateTime? CreateTime { get; set; }

    public int? CreateBy { get; set; }

    public DateTime? ModifyTime { get; set; }

    public int? ModifyBy { get; set; }

    public virtual TblEvent Event { get; set; } = null!;

    public virtual TblUser User { get; set; } = null!;
}
