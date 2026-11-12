using System;
using System.Collections.Generic;

namespace VotingSystem.DataAccess.AppDbContextModels;

public partial class TblEmailRoundRobinState
{
    public int Id { get; set; }

    public long? LastSelectedAccountId { get; set; }

    public int? LastSelectedOrder { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual TblEmailAccount? LastSelectedAccount { get; set; }
}
