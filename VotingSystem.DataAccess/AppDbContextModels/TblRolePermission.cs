using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VotingSystem.DataAccess.AppDbContextModels;

[Table("Tbl_Role_Permission")]
[Index("RoleId", "PermissionId", Name = "UQ_Tbl_RolePermission", IsUnique = true)]
public partial class TblRolePermission
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("role_id")]
    public int RoleId { get; set; }

    [Column("permission_id")]
    public int PermissionId { get; set; }

    [Column("assigned_at")]
    public DateTime AssignedAt { get; set; }

    [Column("assigned_by")]
    public long? AssignedBy { get; set; }

    [Column("delete_flag")]
    public bool DeleteFlag { get; set; }

    [ForeignKey("AssignedBy")]
    [InverseProperty("TblRolePermissions")]
    public virtual TblUser? AssignedByNavigation { get; set; }

    [ForeignKey("PermissionId")]
    [InverseProperty("TblRolePermissions")]
    public virtual TblPermission Permission { get; set; } = null!;

    [ForeignKey("RoleId")]
    [InverseProperty("TblRolePermissions")]
    public virtual TblRole Role { get; set; } = null!;
}
