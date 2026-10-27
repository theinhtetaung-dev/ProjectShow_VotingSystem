using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VotingSystem.DataAccess.AppDbContextModels;

[Table("Tbl_Voting_Limit")]
[Index("UserEventId", Name = "IX_Tbl_Voting_Limit_UserEventId")]
[Index("UserEventId", "ProjectTypeId", Name = "UQ_Tbl_VotingLimit", IsUnique = true)]
public partial class TblVotingLimit
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("user_event_id")]
    public long UserEventId { get; set; }

    [Column("project_type_id")]
    public int ProjectTypeId { get; set; }

    [Column("vote_limit")]
    public int VoteLimit { get; set; }

    [Column("vote_used")]
    public int VoteUsed { get; set; }

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
    [InverseProperty("TblVotingLimitCreatedByNavigations")]
    public virtual TblUser? CreatedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("TblVotingLimitModifiedByNavigations")]
    public virtual TblUser? ModifiedByNavigation { get; set; }

    [ForeignKey("ProjectTypeId")]
    [InverseProperty("TblVotingLimits")]
    public virtual TblProjectType ProjectType { get; set; } = null!;

    [ForeignKey("UserEventId")]
    [InverseProperty("TblVotingLimits")]
    public virtual TblUserEvent UserEvent { get; set; } = null!;
}
