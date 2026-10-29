using VotingSystem.Shared;

namespace VotingSystem.Domain.Features.AuditLog;

public interface IAuditLogService
{
    Task<Result<AuditLogResponseModel>> CreateAuditLogAsync(AuditLogCreateRequestModel request);
    Task<Result<AuditLogResponseModel>> GetAuditLogByID(long id);
    Task<Result<List<AuditLogResponseModel>>> GetAuditLoglist();
}
