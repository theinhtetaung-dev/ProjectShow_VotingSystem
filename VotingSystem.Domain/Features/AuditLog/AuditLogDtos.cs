namespace VotingSystem.Domain.Features.AuditLog;

public class AuditLogCreateRequestModel
{
    public string Action { get; set; } = null!;
    public string EntityType { get; set; } = null!;
    public long? EntityId { get; set; }
    public long? PerformedBy { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? Description { get; set; }
}

public class AuditLogResponseModel
{
    public long Id { get; set; }
    public string Action { get; set; } = null!;
    public string EntityType { get; set; } = null!;
    public long? EntityId { get; set; }
    public long? PerformedBy { get; set; }
    public DateTime PerformedAt { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? Description { get; set; }
}
