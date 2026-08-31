using System;
using System.Collections.Generic;

namespace VotingSystem.DataAccess.AppDbContextModels;

public partial class TblAuditLog
{
    public int Id { get; set; }

    public string Action { get; set; } = null!;

    public string TableName { get; set; } = null!;

    public int RecordId { get; set; }

    public int UserId { get; set; }

    public string? ActionDetails { get; set; }

    public DateTime? CreateTime { get; set; }

    public virtual TblUser User { get; set; } = null!;
}
