using Microsoft.EntityFrameworkCore;
using VotingSystem.DataAccess.AppDbContextModels;
using VotingSystem.Shared;

namespace VotingSystem.Domain.Features.AuditLog;

public class AuditLogService : IAuditLogService
{
    private readonly AppDbContext _dbContext;

    public AuditLogService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region Get Audit Log List
    public async Task<Result<List<AuditLogResponseModel>>> GetAuditLoglist()
    {
        try
        {
            var resModel = await _dbContext.TblAuditLogs
                .OrderByDescending(x => x.Id)
                .Select(auditLog => new AuditLogResponseModel
                {
                    Id = auditLog.Id,
                    Action = auditLog.Action,
                    EntityType = auditLog.EntityType,
                    EntityId = auditLog.EntityId,
                    PerformedBy = auditLog.PerformedBy,
                    PerformedAt = auditLog.PerformedAt,
                    OldValue = auditLog.OldValue,
                    NewValue = auditLog.NewValue,
                    Description = auditLog.Description
                })
                .ToListAsync();

            string message = resModel.Count == 0 ? "No audit logs found." : "Success";
            return Result<List<AuditLogResponseModel>>.Success(resModel, message);
        }
        catch (Exception ex)
        {
            return Result<List<AuditLogResponseModel>>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Get Audit Log By ID
    public async Task<Result<AuditLogResponseModel>> GetAuditLogByID(long id)
    {
        try
        {
            var auditLog = await _dbContext.TblAuditLogs
                .FirstOrDefaultAsync(x => x.Id == id);
            if (auditLog == null)
            {
                return Result<AuditLogResponseModel>.NotFound("Item Not Found!");
            }

            var resModel = new AuditLogResponseModel
            {
                Id = auditLog.Id,
                Action = auditLog.Action,
                EntityType = auditLog.EntityType,
                EntityId = auditLog.EntityId,
                PerformedBy = auditLog.PerformedBy,
                PerformedAt = auditLog.PerformedAt,
                OldValue = auditLog.OldValue,
                NewValue = auditLog.NewValue,
                Description = auditLog.Description
            };
            return Result<AuditLogResponseModel>.Success(resModel, "Success");
        }
        catch (Exception ex)
        {
            return Result<AuditLogResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Create Audit Log
    public async Task<Result<AuditLogResponseModel>> CreateAuditLogAsync(AuditLogCreateRequestModel request)
    {
        try
        {
            if (!request.Action.IsValidRequiredString(100))
            {
                return Result<AuditLogResponseModel>.ValidationError("Action is required and must not exceed 100 characters.");
            }

            if (!request.EntityType.IsValidRequiredString(100))
            {
                return Result<AuditLogResponseModel>.ValidationError("Entity type is required and must not exceed 100 characters.");
            }

            if (request.EntityId.HasValue && !request.EntityId.Value.IsPositive())
            {
                return Result<AuditLogResponseModel>.ValidationError("Entity ID must be greater than zero.");
            }

            if (request.PerformedBy.HasValue && !request.PerformedBy.Value.IsPositive())
            {
                return Result<AuditLogResponseModel>.ValidationError("Performed by user ID must be greater than zero.");
            }

            if (!request.Description.IsValidOptionalString(1000))
            {
                return Result<AuditLogResponseModel>.ValidationError("Description must not exceed 1000 characters.");
            }

            var performedByExists = await _dbContext.TblUsers
                .AnyAsync(x => x.Id == request.PerformedBy && !x.DeleteFlag);
            if (request.PerformedBy.HasValue && !performedByExists)
            {
                return Result<AuditLogResponseModel>.ValidationError("Performed by user not found.");
            }

            var newAuditLog = new TblAuditLog
            {
                Action = request.Action,
                EntityType = request.EntityType,
                EntityId = request.EntityId,
                PerformedBy = request.PerformedBy,
                PerformedAt = DateTime.UtcNow,
                OldValue = request.OldValue,
                NewValue = request.NewValue,
                Description = request.Description
            };
            _dbContext.TblAuditLogs.Add(newAuditLog);
            await _dbContext.SaveChangesAsync();

            var resModel = new AuditLogResponseModel
            {
                Id = newAuditLog.Id,
                Action = newAuditLog.Action,
                EntityType = newAuditLog.EntityType,
                EntityId = newAuditLog.EntityId,
                PerformedBy = newAuditLog.PerformedBy,
                PerformedAt = newAuditLog.PerformedAt,
                OldValue = newAuditLog.OldValue,
                NewValue = newAuditLog.NewValue,
                Description = newAuditLog.Description
            };
            return Result<AuditLogResponseModel>.Success(resModel, "Audit log created successfully.");
        }
        catch (Exception ex)
        {
            return Result<AuditLogResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion
}
