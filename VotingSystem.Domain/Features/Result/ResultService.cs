using Microsoft.EntityFrameworkCore;
using VotingSystem.DataAccess.AppDbContextModels;
using VotingSystem.Shared;

namespace VotingSystem.Domain.Features.Result;

public class ResultService : IResultService
{
    private readonly AppDbContext _dbContext;

    public ResultService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region Get Result List
    public async Task<Result<List<ResultResponseModel>>> GetResultlist()
    {
        try
        {
            var results = await _dbContext.TblResults
                .Where(x => !x.DeleteFlag)
                .ToListAsync();
            var resModel = new List<ResultResponseModel>();

            foreach (var result in results)
            {
                resModel.Add(new ResultResponseModel
                {
                    Id = result.Id,
                    EventId = result.EventId,
                    ProjectId = result.ProjectId,
                    Status = result.Status.ToResultStatus()!.Value,
                    Version = result.Version,
                    ReviewedAt = result.ReviewedAt,
                    ReviewedBy = result.ReviewedBy,
                    PublishedAt = result.PublishedAt,
                    PublishedBy = result.PublishedBy,
                    CreatedAt = result.CreatedAt,
                    CreatedBy = result.CreatedBy,
                    ModifiedAt = result.ModifiedAt,
                    ModifiedBy = result.ModifiedBy
                });
            }

            string message = results.Count == 0 ? "No results found." : "Success";
            return Result<List<ResultResponseModel>>.Success(resModel, message);
        }
        catch (Exception ex)
        {
            return Result<List<ResultResponseModel>>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Get Result By ID
    public async Task<Result<ResultResponseModel>> GetResultByID(long id)
    {
        try
        {
            var result = await _dbContext.TblResults
                .FirstOrDefaultAsync(x => x.Id == id && !x.DeleteFlag);
            if (result == null)
            {
                return Result<ResultResponseModel>.NotFound("Item Not Found!");
            }

            var resModel = new ResultResponseModel
            {
                Id = result.Id,
                EventId = result.EventId,
                ProjectId = result.ProjectId,
                Status = result.Status.ToResultStatus()!.Value,
                Version = result.Version,
                ReviewedAt = result.ReviewedAt,
                ReviewedBy = result.ReviewedBy,
                PublishedAt = result.PublishedAt,
                PublishedBy = result.PublishedBy,
                CreatedAt = result.CreatedAt,
                CreatedBy = result.CreatedBy,
                ModifiedAt = result.ModifiedAt,
                ModifiedBy = result.ModifiedBy
            };
            return Result<ResultResponseModel>.Success(resModel, "Success");
        }
        catch (Exception ex)
        {
            return Result<ResultResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Create Result
    public async Task<Result<ResultResponseModel>> CreateResultAsync(ResultCreateRequestModel request)
    {
        try
        {
            if (!request.EventId.IsPositive())
            {
                return Result<ResultResponseModel>.ValidationError("Event ID must be greater than zero.");
            }

            if (!request.ProjectId.IsPositive())
            {
                return Result<ResultResponseModel>.ValidationError("Project ID must be greater than zero.");
            }

            if (!request.Status.IsValidResultStatus())
            {
                return Result<ResultResponseModel>.ValidationError("Status must be first_prize, second_prize, third_prize, or normal.");
            }

            if (!request.Version.IsPositive())
            {
                return Result<ResultResponseModel>.ValidationError("Version must be greater than zero.");
            }

            if (request.ReviewedBy.HasValue && !request.ReviewedBy.Value.IsPositive())
            {
                return Result<ResultResponseModel>.ValidationError("Reviewed by user ID must be greater than zero.");
            }

            if (request.PublishedBy.HasValue && !request.PublishedBy.Value.IsPositive())
            {
                return Result<ResultResponseModel>.ValidationError("Published by user ID must be greater than zero.");
            }

            var eventExists = await _dbContext.TblEvents
                .AnyAsync(x => x.Id == request.EventId && !x.DeleteFlag);
            if (!eventExists)
            {
                return Result<ResultResponseModel>.ValidationError("Event not found.");
            }

            var project = await _dbContext.TblProjects
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId && !x.DeleteFlag);
            if (project == null)
            {
                return Result<ResultResponseModel>.ValidationError("Project not found.");
            }

            if (project.EventId != request.EventId)
            {
                return Result<ResultResponseModel>.ValidationError("Project does not belong to the selected event.");
            }

            var reviewedByExists = await _dbContext.TblUsers
                .AnyAsync(x => x.Id == request.ReviewedBy && !x.DeleteFlag);
            if (request.ReviewedBy.HasValue && !reviewedByExists)
            {
                return Result<ResultResponseModel>.ValidationError("Reviewed by user not found.");
            }

            var publishedByExists = await _dbContext.TblUsers
                .AnyAsync(x => x.Id == request.PublishedBy && !x.DeleteFlag);
            if (request.PublishedBy.HasValue && !publishedByExists)
            {
                return Result<ResultResponseModel>.ValidationError("Published by user not found.");
            }

            var resultExists = await _dbContext.TblResults
                .AnyAsync(x => x.EventId == request.EventId
                    && x.ProjectId == request.ProjectId
                    && x.Version == request.Version);
            if (resultExists)
            {
                return Result<ResultResponseModel>.ValidationError("Result already exists for this event, project, and version.");
            }

            var newResult = new TblResult
            {
                EventId = request.EventId,
                ProjectId = request.ProjectId,
                Status = request.Status.ToString(),
                Version = request.Version,
                ReviewedAt = request.ReviewedAt,
                ReviewedBy = request.ReviewedBy,
                PublishedAt = request.PublishedAt,
                PublishedBy = request.PublishedBy,
                CreatedAt = DateTime.UtcNow,
                DeleteFlag = false
            };
            _dbContext.TblResults.Add(newResult);
            await _dbContext.SaveChangesAsync();

            var resModel = new ResultResponseModel
            {
                Id = newResult.Id,
                EventId = newResult.EventId,
                ProjectId = newResult.ProjectId,
                Status = newResult.Status.ToResultStatus()!.Value,
                Version = newResult.Version,
                ReviewedAt = newResult.ReviewedAt,
                ReviewedBy = newResult.ReviewedBy,
                PublishedAt = newResult.PublishedAt,
                PublishedBy = newResult.PublishedBy,
                CreatedAt = newResult.CreatedAt,
                CreatedBy = newResult.CreatedBy,
                ModifiedAt = newResult.ModifiedAt,
                ModifiedBy = newResult.ModifiedBy
            };
            return Result<ResultResponseModel>.Success(resModel, "Result created successfully.");
        }
        catch (Exception ex)
        {
            return Result<ResultResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Update Result
    public async Task<Result<ResultResponseModel>> UpdateResult(ResultUpdateRequestModel request)
    {
        try
        {
            var result = await _dbContext.TblResults
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.DeleteFlag);
            if (result == null)
            {
                return Result<ResultResponseModel>.NotFound("Item Not Found!");
            }

            if (request.EventId.HasValue && !request.EventId.Value.IsPositive())
            {
                return Result<ResultResponseModel>.ValidationError("Event ID must be greater than zero.");
            }

            if (request.ProjectId.HasValue && !request.ProjectId.Value.IsPositive())
            {
                return Result<ResultResponseModel>.ValidationError("Project ID must be greater than zero.");
            }

            if (request.Status.HasValue && !request.Status.Value.IsValidResultStatus())
            {
                return Result<ResultResponseModel>.ValidationError("Status must be first_prize, second_prize, third_prize, or normal.");
            }

            if (request.Version.HasValue && !request.Version.Value.IsPositive())
            {
                return Result<ResultResponseModel>.ValidationError("Version must be greater than zero.");
            }

            if (request.ReviewedBy.HasValue && !request.ReviewedBy.Value.IsPositive())
            {
                return Result<ResultResponseModel>.ValidationError("Reviewed by user ID must be greater than zero.");
            }

            if (request.PublishedBy.HasValue && !request.PublishedBy.Value.IsPositive())
            {
                return Result<ResultResponseModel>.ValidationError("Published by user ID must be greater than zero.");
            }

            var eventId = request.EventId ?? result.EventId;
            var projectId = request.ProjectId ?? result.ProjectId;
            var version = request.Version ?? result.Version;
            var relationshipChanged = request.EventId.HasValue || request.ProjectId.HasValue;

            var eventExists = await _dbContext.TblEvents
                .AnyAsync(x => x.Id == eventId && !x.DeleteFlag);
            if (relationshipChanged && !eventExists)
            {
                return Result<ResultResponseModel>.ValidationError("Event not found.");
            }

            var project = await _dbContext.TblProjects
                .FirstOrDefaultAsync(x => x.Id == projectId && !x.DeleteFlag);
            if (relationshipChanged && project == null)
            {
                return Result<ResultResponseModel>.ValidationError("Project not found.");
            }

            if (relationshipChanged && project!.EventId != eventId)
            {
                return Result<ResultResponseModel>.ValidationError("Project does not belong to the selected event.");
            }

            var reviewedByExists = await _dbContext.TblUsers
                .AnyAsync(x => x.Id == request.ReviewedBy && !x.DeleteFlag);
            if (request.ReviewedBy.HasValue && !reviewedByExists)
            {
                return Result<ResultResponseModel>.ValidationError("Reviewed by user not found.");
            }

            var publishedByExists = await _dbContext.TblUsers
                .AnyAsync(x => x.Id == request.PublishedBy && !x.DeleteFlag);
            if (request.PublishedBy.HasValue && !publishedByExists)
            {
                return Result<ResultResponseModel>.ValidationError("Published by user not found.");
            }

            var resultExists = await _dbContext.TblResults
                .AnyAsync(x => x.EventId == eventId
                    && x.ProjectId == projectId
                    && x.Version == version
                    && x.Id != request.Id);
            var uniqueFieldsChanged = request.EventId.HasValue
                || request.ProjectId.HasValue
                || request.Version.HasValue;
            if (uniqueFieldsChanged && resultExists)
            {
                return Result<ResultResponseModel>.ValidationError("Result already exists for this event, project, and version.");
            }

            if (request.EventId.HasValue)
            {
                result.EventId = request.EventId.Value;
            }

            if (request.ProjectId.HasValue)
            {
                result.ProjectId = request.ProjectId.Value;
            }

            if (request.Status.HasValue)
            {
                result.Status = request.Status.Value.ToString();
            }

            if (request.Version.HasValue)
            {
                result.Version = request.Version.Value;
            }

            if (request.ReviewedAt.HasValue)
            {
                result.ReviewedAt = request.ReviewedAt.Value;
            }

            if (request.ReviewedBy.HasValue)
            {
                result.ReviewedBy = request.ReviewedBy.Value;
            }

            if (request.PublishedAt.HasValue)
            {
                result.PublishedAt = request.PublishedAt.Value;
            }

            if (request.PublishedBy.HasValue)
            {
                result.PublishedBy = request.PublishedBy.Value;
            }

            var hasUpdates = request.EventId.HasValue
                || request.ProjectId.HasValue
                || request.Status.HasValue
                || request.Version.HasValue
                || request.ReviewedAt.HasValue
                || request.ReviewedBy.HasValue
                || request.PublishedAt.HasValue
                || request.PublishedBy.HasValue;
            if (hasUpdates)
            {
                result.ModifiedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }

            var resModel = new ResultResponseModel
            {
                Id = result.Id,
                EventId = result.EventId,
                ProjectId = result.ProjectId,
                Status = result.Status.ToResultStatus()!.Value,
                Version = result.Version,
                ReviewedAt = result.ReviewedAt,
                ReviewedBy = result.ReviewedBy,
                PublishedAt = result.PublishedAt,
                PublishedBy = result.PublishedBy,
                CreatedAt = result.CreatedAt,
                CreatedBy = result.CreatedBy,
                ModifiedAt = result.ModifiedAt,
                ModifiedBy = result.ModifiedBy
            };
            return Result<ResultResponseModel>.Success(resModel, "Result updated successfully.");
        }
        catch (Exception ex)
        {
            return Result<ResultResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion
}
