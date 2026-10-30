using System;
using System.Collections.Generic;

namespace VotingSystem.DataAccess.AppDbContextModels;

public partial class TblRolePermission
{
    public long Id { get; set; }

    public int RoleId { get; set; }

    public int PermissionId { get; set; }

    public DateTime AssignedAt { get; set; }

    public long? AssignedBy { get; set; }

    public bool DeleteFlag { get; set; }

    public virtual TblUser? AssignedByNavigation { get; set; }

    public virtual TblPermission Permission { get; set; } = null!;

    public virtual TblRole Role { get; set; } = null!;
}
