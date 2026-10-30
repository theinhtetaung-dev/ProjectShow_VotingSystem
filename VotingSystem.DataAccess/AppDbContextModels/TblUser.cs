using System;
using System.Collections.Generic;

namespace VotingSystem.DataAccess.AppDbContextModels;

public partial class TblUser
{
    public long Id { get; set; }

    public string Email { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? PasswordHash { get; set; }

    public int RoleId { get; set; }

    public bool IsActive { get; set; }

    public bool DeleteFlag { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public long? ModifiedBy { get; set; }

    public virtual TblRole Role { get; set; } = null!;

    public virtual ICollection<TblAdminEvent> TblAdminEventAdminUsers { get; set; } = new List<TblAdminEvent>();

    public virtual ICollection<TblAdminEvent> TblAdminEventAssignedByNavigations { get; set; } = new List<TblAdminEvent>();

    public virtual ICollection<TblAuditLog> TblAuditLogs { get; set; } = new List<TblAuditLog>();

    public virtual ICollection<TblProject> TblProjectCreatedByNavigations { get; set; } = new List<TblProject>();

    public virtual ICollection<TblProject> TblProjectModifiedByNavigations { get; set; } = new List<TblProject>();

    public virtual ICollection<TblRefreshToken> TblRefreshTokens { get; set; } = new List<TblRefreshToken>();

    public virtual ICollection<TblResult> TblResultCreatedByNavigations { get; set; } = new List<TblResult>();

    public virtual ICollection<TblResult> TblResultModifiedByNavigations { get; set; } = new List<TblResult>();

    public virtual ICollection<TblResult> TblResultPublishedByNavigations { get; set; } = new List<TblResult>();

    public virtual ICollection<TblResult> TblResultReviewedByNavigations { get; set; } = new List<TblResult>();

    public virtual ICollection<TblRolePermission> TblRolePermissions { get; set; } = new List<TblRolePermission>();

    public virtual ICollection<TblUserEvent> TblUserEventAssignedByNavigations { get; set; } = new List<TblUserEvent>();

    public virtual ICollection<TblUserEvent> TblUserEventRemovedByNavigations { get; set; } = new List<TblUserEvent>();

    public virtual ICollection<TblUserEvent> TblUserEventUsers { get; set; } = new List<TblUserEvent>();

    public virtual ICollection<TblVotingLimit> TblVotingLimitCreatedByNavigations { get; set; } = new List<TblVotingLimit>();

    public virtual ICollection<TblVotingLimit> TblVotingLimitModifiedByNavigations { get; set; } = new List<TblVotingLimit>();
}
