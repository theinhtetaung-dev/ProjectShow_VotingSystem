using System;
using System.Collections.Generic;

namespace VotingSystem.DataAccess.AppDbContextModels;

public partial class TblUser
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool? DeleteFlag { get; set; }

    public DateTime? CreateTime { get; set; }

    public int? CreateBy { get; set; }

    public DateTime? ModifyTime { get; set; }

    public int? ModifyBy { get; set; }

    public virtual TblRole Role { get; set; } = null!;

    public virtual ICollection<TblAuditLog> TblAuditLogs { get; set; } = new List<TblAuditLog>();

    public virtual ICollection<TblEventAdmin> TblEventAdmins { get; set; } = new List<TblEventAdmin>();

    public virtual ICollection<TblEventUser> TblEventUsers { get; set; } = new List<TblEventUser>();

    public virtual ICollection<TblUserVoteLimit> TblUserVoteLimits { get; set; } = new List<TblUserVoteLimit>();

    public virtual ICollection<TblVote> TblVotes { get; set; } = new List<TblVote>();
}
