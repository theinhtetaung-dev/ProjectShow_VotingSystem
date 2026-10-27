using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VotingSystem.DataAccess.AppDbContextModels;

[Table("Tbl_User_Event")]
[Index("EventId", Name = "IX_Tbl_User_Event_EventId")]
[Index("UserId", Name = "IX_Tbl_User_Event_UserId")]
[Index("UserId", "EventId", Name = "UQ_Tbl_UserEvent", IsUnique = true)]
public partial class TblUserEvent
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("user_id")]
    public long UserId { get; set; }

    [Column("event_id")]
    public long EventId { get; set; }

    [Column("event_password_hash")]
    [StringLength(500)]
    public string EventPasswordHash { get; set; } = null!;

    [Column("assigned_at")]
    public DateTime AssignedAt { get; set; }

    [Column("assigned_by")]
    public long? AssignedBy { get; set; }

    [Column("password_generated_at")]
    public DateTime PasswordGeneratedAt { get; set; }

    [Column("password_expires_at")]
    public DateTime PasswordExpiresAt { get; set; }

    [Column("access_status")]
    [StringLength(20)]
    public string AccessStatus { get; set; } = null!;

    [Column("removed_at")]
    public DateTime? RemovedAt { get; set; }

    [Column("removed_by")]
    public long? RemovedBy { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("delete_flag")]
    public bool DeleteFlag { get; set; }

    [ForeignKey("AssignedBy")]
    [InverseProperty("TblUserEventAssignedByNavigations")]
    public virtual TblUser? AssignedByNavigation { get; set; }

    [ForeignKey("EventId")]
    [InverseProperty("TblUserEvents")]
    public virtual TblEvent Event { get; set; } = null!;

    [ForeignKey("RemovedBy")]
    [InverseProperty("TblUserEventRemovedByNavigations")]
    public virtual TblUser? RemovedByNavigation { get; set; }

    [InverseProperty("UserEvent")]
    public virtual ICollection<TblVote> TblVotes { get; set; } = new List<TblVote>();

    [InverseProperty("UserEvent")]
    public virtual ICollection<TblVotingLimit> TblVotingLimits { get; set; } = new List<TblVotingLimit>();

    [ForeignKey("UserId")]
    [InverseProperty("TblUserEventUsers")]
    public virtual TblUser User { get; set; } = null!;
}
