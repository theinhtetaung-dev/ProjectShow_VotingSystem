using System;
using System.Collections.Generic;

namespace VotingSystem.DataAccess.AppDbContextModels;

public partial class TblAdminEvent
{
    public long Id { get; set; }

    public long AdminUserId { get; set; }

    public long EventId { get; set; }

    public DateTime AssignedAt { get; set; }

    public long? AssignedBy { get; set; }

    public bool IsActive { get; set; }

    public bool DeleteFlag { get; set; }

    public virtual TblUser AdminUser { get; set; } = null!;

    public virtual TblUser? AssignedByNavigation { get; set; }

    public virtual TblEvent Event { get; set; } = null!;
}
