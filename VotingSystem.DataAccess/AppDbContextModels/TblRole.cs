using System;
using System.Collections.Generic;

namespace VotingSystem.DataAccess.AppDbContextModels;

public partial class TblRole
{
    public int Id { get; set; }

    public string RoleName { get; set; } = null!;

    public bool? DeleteFlag { get; set; }

    public DateTime? CreateTime { get; set; }

    public int? CreateBy { get; set; }

    public DateTime? ModifyTime { get; set; }

    public int? ModifyBy { get; set; }

    public virtual ICollection<TblUser> TblUsers { get; set; } = new List<TblUser>();

    public virtual ICollection<TblPermission> Permissions { get; set; } = new List<TblPermission>();
}
