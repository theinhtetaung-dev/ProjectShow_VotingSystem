using System;
using System.Collections.Generic;

namespace VotingSystem.DataAccess.AppDbContextModels;

public partial class TblPermission
{
    public int Id { get; set; }

    public string PermissionName { get; set; } = null!;

    public bool? DeleteFlag { get; set; }

    public DateTime? CreateTime { get; set; }

    public int? CreateBy { get; set; }

    public DateTime? ModifyTime { get; set; }

    public int? ModifyBy { get; set; }

    public virtual ICollection<TblRole> Roles { get; set; } = new List<TblRole>();
}
