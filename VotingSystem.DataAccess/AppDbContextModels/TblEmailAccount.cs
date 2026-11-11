using System;
using System.Collections.Generic;

namespace VotingSystem.DataAccess.AppDbContextModels;

public partial class TblEmailAccount
{
    public long Id { get; set; }

    public string EmailAddress { get; set; } = null!;

    public string? DisplayName { get; set; }

    public string EncryptedAppPassword { get; set; } = null!;

    public int PriorityOrder { get; set; }

    public bool IsActive { get; set; }

    public int TotalEmailsSent { get; set; }

    public DateTime? LastUsedAt { get; set; }

    public bool DeleteFlag { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public long? ModifiedBy { get; set; }

    public virtual TblUser? CreatedByNavigation { get; set; }

    public virtual TblUser? ModifiedByNavigation { get; set; }

    public virtual ICollection<TblEmailRoundRobinState> TblEmailRoundRobinStates { get; set; } = new List<TblEmailRoundRobinState>();
}
