using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VotingSystem.DataAccess.AppDbContextModels;

[Table("Tbl_User")]
[Index("RoleId", Name = "IX_Tbl_User_RoleId")]
[Index("Email", Name = "UQ_Tbl_User_Email", IsUnique = true)]
public partial class TblUser
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("email")]
    [StringLength(255)]
    public string Email { get; set; } = null!;

    [Column("name")]
    [StringLength(150)]
    public string Name { get; set; } = null!;

    [Column("password_hash")]
    [StringLength(500)]
    public string? PasswordHash { get; set; }

    [Column("role_id")]
    public int RoleId { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("delete_flag")]
    public bool DeleteFlag { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("created_by")]
    public long? CreatedBy { get; set; }

    [Column("modified_at")]
    public DateTime? ModifiedAt { get; set; }

    [Column("modified_by")]
    public long? ModifiedBy { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("TblUsers")]
    public virtual TblRole Role { get; set; } = null!;

    [InverseProperty("AdminUser")]
    public virtual ICollection<TblAdminEvent> TblAdminEventAdminUsers { get; set; } = new List<TblAdminEvent>();

    [InverseProperty("AssignedByNavigation")]
    public virtual ICollection<TblAdminEvent> TblAdminEventAssignedByNavigations { get; set; } = new List<TblAdminEvent>();

    [InverseProperty("PerformedByNavigation")]
    public virtual ICollection<TblAuditLog> TblAuditLogs { get; set; } = new List<TblAuditLog>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<TblProject> TblProjectCreatedByNavigations { get; set; } = new List<TblProject>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<TblProject> TblProjectModifiedByNavigations { get; set; } = new List<TblProject>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<TblResult> TblResultCreatedByNavigations { get; set; } = new List<TblResult>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<TblResult> TblResultModifiedByNavigations { get; set; } = new List<TblResult>();

    [InverseProperty("PublishedByNavigation")]
    public virtual ICollection<TblResult> TblResultPublishedByNavigations { get; set; } = new List<TblResult>();

    [InverseProperty("ReviewedByNavigation")]
    public virtual ICollection<TblResult> TblResultReviewedByNavigations { get; set; } = new List<TblResult>();

    [InverseProperty("AssignedByNavigation")]
    public virtual ICollection<TblRolePermission> TblRolePermissions { get; set; } = new List<TblRolePermission>();

    [InverseProperty("AssignedByNavigation")]
    public virtual ICollection<TblUserEvent> TblUserEventAssignedByNavigations { get; set; } = new List<TblUserEvent>();

    [InverseProperty("RemovedByNavigation")]
    public virtual ICollection<TblUserEvent> TblUserEventRemovedByNavigations { get; set; } = new List<TblUserEvent>();

    [InverseProperty("User")]
    public virtual ICollection<TblUserEvent> TblUserEventUsers { get; set; } = new List<TblUserEvent>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<TblVotingLimit> TblVotingLimitCreatedByNavigations { get; set; } = new List<TblVotingLimit>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<TblVotingLimit> TblVotingLimitModifiedByNavigations { get; set; } = new List<TblVotingLimit>();
}
