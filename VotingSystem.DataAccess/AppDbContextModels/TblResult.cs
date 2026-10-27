using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VotingSystem.DataAccess.AppDbContextModels;

[Table("Tbl_Result")]
[Index("EventId", Name = "IX_Tbl_Result_EventId")]
[Index("ProjectId", Name = "IX_Tbl_Result_ProjectId")]
[Index("EventId", "ProjectId", "Version", Name = "UQ_Tbl_Result", IsUnique = true)]
public partial class TblResult
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("event_id")]
    public long EventId { get; set; }

    [Column("project_id")]
    public long ProjectId { get; set; }

    [Column("status")]
    [StringLength(30)]
    public string Status { get; set; } = null!;

    [Column("version")]
    public int Version { get; set; }

    [Column("reviewed_at")]
    public DateTime? ReviewedAt { get; set; }

    [Column("reviewed_by")]
    public long? ReviewedBy { get; set; }

    [Column("published_at")]
    public DateTime? PublishedAt { get; set; }

    [Column("published_by")]
    public long? PublishedBy { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("created_by")]
    public long? CreatedBy { get; set; }

    [Column("modified_at")]
    public DateTime? ModifiedAt { get; set; }

    [Column("modified_by")]
    public long? ModifiedBy { get; set; }

    [Column("delete_flag")]
    public bool DeleteFlag { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("TblResultCreatedByNavigations")]
    public virtual TblUser? CreatedByNavigation { get; set; }

    [ForeignKey("EventId")]
    [InverseProperty("TblResults")]
    public virtual TblEvent Event { get; set; } = null!;

    [ForeignKey("ModifiedBy")]
    [InverseProperty("TblResultModifiedByNavigations")]
    public virtual TblUser? ModifiedByNavigation { get; set; }

    [ForeignKey("ProjectId")]
    [InverseProperty("TblResults")]
    public virtual TblProject Project { get; set; } = null!;

    [ForeignKey("PublishedBy")]
    [InverseProperty("TblResultPublishedByNavigations")]
    public virtual TblUser? PublishedByNavigation { get; set; }

    [ForeignKey("ReviewedBy")]
    [InverseProperty("TblResultReviewedByNavigations")]
    public virtual TblUser? ReviewedByNavigation { get; set; }
}
