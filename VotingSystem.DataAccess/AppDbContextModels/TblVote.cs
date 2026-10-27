using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VotingSystem.DataAccess.AppDbContextModels;

[Table("Tbl_Vote")]
[Index("ProjectId", Name = "IX_Tbl_Vote_ProjectId")]
[Index("UserEventId", Name = "IX_Tbl_Vote_UserEventId")]
[Index("UserEventId", "ProjectId", Name = "UQ_Tbl_Vote_UserEvent_Project", IsUnique = true)]
public partial class TblVote
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("user_event_id")]
    public long UserEventId { get; set; }

    [Column("project_id")]
    public long ProjectId { get; set; }

    [Column("voted_at")]
    public DateTime VotedAt { get; set; }

    [ForeignKey("ProjectId")]
    [InverseProperty("TblVotes")]
    public virtual TblProject Project { get; set; } = null!;

    [ForeignKey("UserEventId")]
    [InverseProperty("TblVotes")]
    public virtual TblUserEvent UserEvent { get; set; } = null!;
}
