using System;
using System.Collections.Generic;

namespace VotingSystem.DataAccess.AppDbContextModels;

public partial class TblAuditLog
{
    public long Id { get; set; }

    public string Action { get; set; } = null!;

    public string EntityType { get; set; } = null!;

    public long? EntityId { get; set; }

    public long? PerformedBy { get; set; }

    public DateTime PerformedAt { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public string? Description { get; set; }

    public virtual TblUser? PerformedByNavigation { get; set; }
}
