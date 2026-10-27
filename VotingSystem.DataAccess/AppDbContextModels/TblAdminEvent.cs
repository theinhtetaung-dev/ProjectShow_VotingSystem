using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VotingSystem.DataAccess.AppDbContextModels;

[Table("Tbl_Admin_Event")]
[Index("AdminUserId", Name = "IX_Tbl_Admin_Event_AdminUserId")]
[Index("EventId", Name = "IX_Tbl_Admin_Event_EventId")]
[Index("AdminUserId", "EventId", Name = "UQ_Tbl_AdminEvent", IsUnique = true)]
public partial class TblAdminEvent
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("admin_user_id")]
    public long AdminUserId { get; set; }

    [Column("event_id")]
    public long EventId { get; set; }

    [Column("assigned_at")]
    public DateTime AssignedAt { get; set; }

    [Column("assigned_by")]
    public long? AssignedBy { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("delete_flag")]
    public bool DeleteFlag { get; set; }

    [ForeignKey("AdminUserId")]
    [InverseProperty("TblAdminEventAdminUsers")]
    public virtual TblUser AdminUser { get; set; } = null!;

    [ForeignKey("AssignedBy")]
    [InverseProperty("TblAdminEventAssignedByNavigations")]
    public virtual TblUser? AssignedByNavigation { get; set; }

    [ForeignKey("EventId")]
    [InverseProperty("TblAdminEvents")]
    public virtual TblEvent Event { get; set; } = null!;
}
