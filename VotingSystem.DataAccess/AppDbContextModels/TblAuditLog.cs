using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VotingSystem.DataAccess.AppDbContextModels;

[Table("Tbl_Audit_Log")]
[Index("EntityType", "EntityId", Name = "IX_Tbl_Audit_Log_Entity")]
[Index("PerformedBy", Name = "IX_Tbl_Audit_Log_PerformedBy")]
public partial class TblAuditLog
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("action")]
    [StringLength(100)]
    public string Action { get; set; } = null!;

    [Column("entity_type")]
    [StringLength(100)]
    public string EntityType { get; set; } = null!;

    [Column("entity_id")]
    public long? EntityId { get; set; }

    [Column("performed_by")]
    public long? PerformedBy { get; set; }

    [Column("performed_at")]
    public DateTime PerformedAt { get; set; }

    [Column("old_value")]
    public string? OldValue { get; set; }

    [Column("new_value")]
    public string? NewValue { get; set; }

    [Column("description")]
    [StringLength(1000)]
    public string? Description { get; set; }

    [ForeignKey("PerformedBy")]
    [InverseProperty("TblAuditLogs")]
    public virtual TblUser? PerformedByNavigation { get; set; }
}
