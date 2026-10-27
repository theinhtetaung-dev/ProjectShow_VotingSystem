using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VotingSystem.DataAccess.AppDbContextModels;

[Table("Tbl_Project")]
[Index("EventId", Name = "IX_Tbl_Project_EventId")]
[Index("ProjectTypeId", Name = "IX_Tbl_Project_ProjectTypeId")]
public partial class TblProject
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("event_id")]
    public long EventId { get; set; }

    [Column("project_type_id")]
    public int ProjectTypeId { get; set; }

    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [Column("description")]
    [StringLength(1000)]
    public string? Description { get; set; }

    [Column("vote_counts")]
    public int VoteCounts { get; set; }

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

    [ForeignKey("CreatedBy")]
    [InverseProperty("TblProjectCreatedByNavigations")]
    public virtual TblUser? CreatedByNavigation { get; set; }

    [ForeignKey("EventId")]
    [InverseProperty("TblProjects")]
    public virtual TblEvent Event { get; set; } = null!;

    [ForeignKey("ModifiedBy")]
    [InverseProperty("TblProjectModifiedByNavigations")]
    public virtual TblUser? ModifiedByNavigation { get; set; }

    [ForeignKey("ProjectTypeId")]
    [InverseProperty("TblProjects")]
    public virtual TblProjectType ProjectType { get; set; } = null!;

    [InverseProperty("Project")]
    public virtual ICollection<TblResult> TblResults { get; set; } = new List<TblResult>();

    [InverseProperty("Project")]
    public virtual ICollection<TblVote> TblVotes { get; set; } = new List<TblVote>();
}
