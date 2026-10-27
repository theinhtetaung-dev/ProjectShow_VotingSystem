using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VotingSystem.DataAccess.AppDbContextModels;

[Table("Tbl_Event")]
[Index("Code", Name = "UQ_Tbl_Event_Code", IsUnique = true)]
public partial class TblEvent
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [Column("code")]
    [StringLength(100)]
    public string Code { get; set; } = null!;

    [Column("description")]
    [StringLength(1000)]
    public string? Description { get; set; }

    [Column("event_start_at")]
    public DateTime EventStartAt { get; set; }

    [Column("event_end_at")]
    public DateTime EventEndAt { get; set; }

    [Column("voting_start_at")]
    public DateTime VotingStartAt { get; set; }

    [Column("voting_end_at")]
    public DateTime VotingEndAt { get; set; }

    [Column("result_show_start_at")]
    public DateTime? ResultShowStartAt { get; set; }

    [Column("result_show_end_at")]
    public DateTime? ResultShowEndAt { get; set; }

    [Column("status")]
    [StringLength(30)]
    public string Status { get; set; } = null!;

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

    [InverseProperty("Event")]
    public virtual ICollection<TblAdminEvent> TblAdminEvents { get; set; } = new List<TblAdminEvent>();

    [InverseProperty("Event")]
    public virtual ICollection<TblProject> TblProjects { get; set; } = new List<TblProject>();

    [InverseProperty("Event")]
    public virtual ICollection<TblResult> TblResults { get; set; } = new List<TblResult>();

    [InverseProperty("Event")]
    public virtual ICollection<TblUserEvent> TblUserEvents { get; set; } = new List<TblUserEvent>();
}
