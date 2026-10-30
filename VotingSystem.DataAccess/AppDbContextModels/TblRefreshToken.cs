using System;
using System.Collections.Generic;

namespace VotingSystem.DataAccess.AppDbContextModels;

public partial class TblRefreshToken
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string TokenHash { get; set; } = null!;

    public Guid TokenFamilyId { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastUsedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public long? ReplacedByTokenId { get; set; }

    public string? CreatedByIp { get; set; }

    public string? RevokedByIp { get; set; }

    public string? DeviceName { get; set; }

    public string? RevocationReason { get; set; }

    public virtual ICollection<TblRefreshToken> InverseReplacedByToken { get; set; } = new List<TblRefreshToken>();

    public virtual TblRefreshToken? ReplacedByToken { get; set; }

    public virtual TblUser User { get; set; } = null!;
}
