using System.Data;
using Microsoft.EntityFrameworkCore;
using VotingSystem.DataAccess.AppDbContextModels;
using VotingSystem.Shared;

namespace VotingSystem.Domain.Features.Voting;

public class VotingService : IVotingService
{
    private readonly AppDbContext _dbContext;

    public VotingService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region Get Voting List
    public async Task<Result<List<VotingResponseModel>>> GetVotinglist()
    {
        try
        {
            var resModel = await _dbContext.TblVotes
                .OrderBy(x => x.Id)
                .Select(vote => new VotingResponseModel
                {
                    Id = vote.Id,
                    UserEventId = vote.UserEventId,
                    ProjectId = vote.ProjectId,
                    VotedAt = vote.VotedAt
                })
                .ToListAsync();

            string message = resModel.Count == 0 ? "No votes found." : "Success";
            return Result<List<VotingResponseModel>>.Success(resModel, message);
        }
        catch (Exception ex)
        {
            return Result<List<VotingResponseModel>>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Update Voting Limit
    public async Task<Result<VotingLimitResponseModel>> UpdateVotingLimitAsync(VotingLimitUpdateRequestModel request)
    {
        try
        {
            if (!request.UserEventId.IsPositive())
            {
                return Result<VotingLimitResponseModel>.ValidationError("User event ID must be greater than zero.");
            }

            if (!request.ProjectTypeId.IsPositive())
            {
                return Result<VotingLimitResponseModel>.ValidationError("Project type ID must be greater than zero.");
            }

            if (!request.VoteLimit.IsNonNegative())
            {
                return Result<VotingLimitResponseModel>.ValidationError("Vote limit must be zero or greater.");
            }

            var userEvent = await _dbContext.TblUserEvents
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.UserEventId && !x.DeleteFlag);
            if (userEvent == null)
            {
                return Result<VotingLimitResponseModel>.ValidationError("User event not found.");
            }

            if (request.EventId.HasValue && userEvent.EventId != request.EventId.Value)
            {
                return Result<VotingLimitResponseModel>.ValidationError("User does not belong to the selected event.");
            }

            var projectTypeExists = await _dbContext.TblProjectTypes
                .AnyAsync(x => x.Id == request.ProjectTypeId && !x.DeleteFlag);
            if (!projectTypeExists)
            {
                return Result<VotingLimitResponseModel>.ValidationError("Project type not found.");
            }

            var votingLimit = await _dbContext.TblVotingLimits
                .FirstOrDefaultAsync(x => x.UserEventId == request.UserEventId
                    && x.ProjectTypeId == request.ProjectTypeId);

            if (votingLimit != null && request.VoteLimit < votingLimit.VoteUsed)
            {
                return Result<VotingLimitResponseModel>.ValidationError("Vote limit cannot be less than votes already used.");
            }

            if (votingLimit == null)
            {
                votingLimit = new TblVotingLimit
                {
                    UserEventId = request.UserEventId,
                    ProjectTypeId = request.ProjectTypeId,
                    VoteLimit = request.VoteLimit,
                    VoteUsed = 0,
                    CreatedAt = DateTime.UtcNow,
                    DeleteFlag = false
                };
                _dbContext.TblVotingLimits.Add(votingLimit);
            }
            else
            {
                votingLimit.VoteLimit = request.VoteLimit;
                votingLimit.ModifiedAt = DateTime.UtcNow;
                votingLimit.DeleteFlag = false;
            }

            await _dbContext.SaveChangesAsync();

            var resModel = new VotingLimitResponseModel
            {
                Id = votingLimit.Id,
                UserEventId = votingLimit.UserEventId,
                ProjectTypeId = votingLimit.ProjectTypeId,
                VoteLimit = votingLimit.VoteLimit,
                VoteUsed = votingLimit.VoteUsed
            };
            return Result<VotingLimitResponseModel>.Success(resModel, "Voting limit updated successfully.");
        }
        catch (Exception ex)
        {
            return Result<VotingLimitResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Vote
    public async Task<Result<VoteResponseModel>> VoteAsync(VoteRequestModel request, long authenticatedUserId)
    {
        try
        {
            await using var transaction = await _dbContext.Database
                .BeginTransactionAsync(IsolationLevel.Serializable);

            if (!request.UserEventId.IsPositive())
            {
                return Result<VoteResponseModel>.ValidationError("User event ID must be greater than zero.");
            }

            if (!request.ProjectId.IsPositive())
            {
                return Result<VoteResponseModel>.ValidationError("Project ID must be greater than zero.");
            }

            var userEvent = await _dbContext.TblUserEvents
                .FirstOrDefaultAsync(x => x.Id == request.UserEventId
                    && x.UserId == authenticatedUserId
                    && !x.DeleteFlag);
            if (userEvent == null)
            {
                return Result<VoteResponseModel>.ValidationError("User event not found.");
            }

            var currentTime = DateTime.UtcNow;
            if (!userEvent.IsActive
                || userEvent.AccessStatus != "valid"
                || userEvent.PasswordExpiresAt < currentTime)
            {
                return Result<VoteResponseModel>.ValidationError("User event is inactive or expired.");
            }

            var project = await _dbContext.TblProjects
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId
                    && x.IsActive
                    && !x.DeleteFlag);
            if (project == null)
            {
                return Result<VoteResponseModel>.ValidationError("Project not found.");
            }

            if (project.EventId != userEvent.EventId)
            {
                return Result<VoteResponseModel>.ValidationError("Project does not belong to the user's event.");
            }

            var votingEvent = await _dbContext.TblEvents
                .FirstOrDefaultAsync(x => x.Id == userEvent.EventId
                    && x.IsActive
                    && !x.DeleteFlag);
            if (votingEvent == null)
            {
                return Result<VoteResponseModel>.ValidationError("Event not found.");
            }

            if (currentTime < votingEvent.VotingStartAt || currentTime > votingEvent.VotingEndAt)
            {
                return Result<VoteResponseModel>.ValidationError("Voting is not currently open.");
            }

            var votingLimit = await _dbContext.TblVotingLimits
                .FirstOrDefaultAsync(x => x.UserEventId == request.UserEventId
                    && x.ProjectTypeId == project.ProjectTypeId
                    && !x.DeleteFlag);
            if (votingLimit == null)
            {
                return Result<VoteResponseModel>.ValidationError("Voting limit not found.");
            }

            if (votingLimit.VoteUsed >= votingLimit.VoteLimit)
            {
                return Result<VoteResponseModel>.ValidationError("Voting limit has been reached.");
            }

            var voteExists = await _dbContext.TblVotes
                .AnyAsync(x => x.UserEventId == request.UserEventId
                    && x.ProjectId == request.ProjectId);
            if (voteExists)
            {
                return Result<VoteResponseModel>.ValidationError("User has already voted for this project.");
            }

            var vote = new TblVote
            {
                UserEventId = request.UserEventId,
                ProjectId = request.ProjectId,
                VotedAt = currentTime
            };
            votingLimit.VoteUsed++;
            votingLimit.ModifiedAt = currentTime;
            project.VoteCounts++;
            project.ModifiedAt = currentTime;
            _dbContext.TblVotes.Add(vote);

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            var resModel = new VoteResponseModel
            {
                Id = vote.Id,
                UserEventId = vote.UserEventId,
                ProjectId = vote.ProjectId,
                VotedAt = vote.VotedAt,
                VoteLimit = votingLimit.VoteLimit,
                VoteUsed = votingLimit.VoteUsed,
                ProjectVoteCounts = project.VoteCounts
            };
            return Result<VoteResponseModel>.Success(resModel, "Voting successful.");
        }
        catch (Exception ex)
        {
            return Result<VoteResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion
}
