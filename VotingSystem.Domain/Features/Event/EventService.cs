using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VotingSystem.DataAccess.AppDbContextModels;
using VotingSystem.Shared;
namespace VotingSystem.Domain.Features.Event;

public class EventService
{
    private readonly AppDbContext _dbContext;

    public EventService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    #region Get Event List
    public async Task<Result<List<EventResponseModel>>> GetEventlist()
    {
        try
        {
            var events = await _dbContext.TblEvents
                .Where(x => !x.DeleteFlag)
                .ToListAsync();
            var resModel = new List<EventResponseModel>();

            foreach (var ev in events)
            {
                resModel.Add(new EventResponseModel
                {
                    Id = ev.Id,
                    Name = ev.Name,
                    Code = ev.Code,
                    Description = ev.Description,
                    EventStartAt = ev.EventStartAt,
                    EventEndAt = ev.EventEndAt,
                    VotingStartAt = ev.VotingStartAt,
                    VotingEndAt = ev.VotingEndAt,
                    ResultShowStartAt = ev.ResultShowStartAt,
                    ResultShowEndAt = ev.ResultShowEndAt,
                    Status = ev.Status
                });
            }
            string message = events.Count == 0 ? "No events found." : "Success";

            return Result<List<EventResponseModel>>.Success(resModel, message);
        }
        catch (Exception ex)
        {
            return Result<List<EventResponseModel>>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Get Event By ID
    public async Task<Result<EventResponseModel>> GetEventByID(long id)
    {
        try
        {
            var _event = await _dbContext.TblEvents.FirstOrDefaultAsync(x => x.Id == id && !x.DeleteFlag);
            if(_event == null)
            {
                return Result<EventResponseModel>.NotFound("Item Not Found!");
            }

            var resModel = new EventResponseModel()
            {
                Id = _event.Id,
                Name = _event.Name,
                Code = _event.Code,
                Description = _event.Description,
                EventStartAt = _event.EventStartAt,
                EventEndAt = _event.EventEndAt,
                VotingStartAt = _event.VotingStartAt,
                VotingEndAt = _event.VotingEndAt,
                ResultShowStartAt = _event.ResultShowStartAt,
                ResultShowEndAt = _event.ResultShowEndAt,
                Status = _event.Status
            };
            return Result<EventResponseModel>.Success(resModel, "Success");
        }

        catch (Exception ex)
        {
            return Result<EventResponseModel>.SystemError(ex.Message);
        }
    }

    #endregion

    #region Create Event
    public async Task<Result<EventResponseModel>> CreateEventAsync(EventCreateRequestModel request)
    {
        try
        {
            var eventStatus = request.Status.ToEventStatus();
            if (!request.Name.IsValidEventName())
            {
                return Result<EventResponseModel>.ValidationError("Name is required and must not exceed 200 characters.");
            }

            if (!request.Description.IsValidDescription())
            {
                return Result<EventResponseModel>.ValidationError("Description must not exceed 1000 characters.");
            }

            if (!request.EndDate.IsValidEventDate(request.StartDate))
            {
                return Result<EventResponseModel>.ValidationError("Event end date must be later than event start date.");
            }

            if (!request.VotingEndAt.IsValidVotingDate(request.VotingStartAt))
            {
                return Result<EventResponseModel>.ValidationError("Voting end date must be later than voting start date.");
            }

            if (!request.VotingStartAt.IsVotingInsideEvent(
                    request.VotingEndAt,
                    request.StartDate,
                    request.EndDate))
            {
                return Result<EventResponseModel>.ValidationError("Voting dates must be within the event date range.");
            }

            if (!request.ResultShowEndAt.IsValidResultShowDate(request.ResultShowStartAt))
            {
                return Result<EventResponseModel>.ValidationError("Result show end date must be later than result show start date.");
            }

            if (!eventStatus.IsValidEventStatus())
            {
                return Result<EventResponseModel>.ValidationError("Status must be Upcoming, Active, Ended, or Published.");
            }

            if (!request.Code.IsValidEventCode())
            {
                return Result<EventResponseModel>.ValidationError("Code is required and must not exceed 100 characters.");
            }

            if (await _dbContext.TblEvents.AnyAsync(x => x.Code == request.Code))
            {
                return Result<EventResponseModel>.ValidationError("Event code already exists.");
            }

            var newEvent = new TblEvent
            {
                Name = request.Name,
                Code = request.Code,
                Description = request.Description,
                EventStartAt = request.StartDate,
                EventEndAt = request.EndDate,
                VotingStartAt = request.VotingStartAt,
                VotingEndAt = request.VotingEndAt,
                ResultShowStartAt = request.ResultShowStartAt,
                ResultShowEndAt = request.ResultShowEndAt,
                Status = eventStatus!.Value.ToString(),
                IsActive = true,
                DeleteFlag = false,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.TblEvents.Add(newEvent);
            await _dbContext.SaveChangesAsync();

            var resModel = new EventResponseModel
            {
                Id = newEvent.Id,
                Name = newEvent.Name,
                Code = newEvent.Code,
                Description = newEvent.Description,
                EventStartAt = newEvent.EventStartAt,
                EventEndAt = newEvent.EventEndAt,
                VotingStartAt = newEvent.VotingStartAt,
                VotingEndAt = newEvent.VotingEndAt,
                ResultShowStartAt = newEvent.ResultShowStartAt,
                ResultShowEndAt = newEvent.ResultShowEndAt,
                Status = newEvent.Status
            };
            return Result<EventResponseModel>.Success(resModel, "Event created successfully.");
        }
        catch (Exception ex)
        {
            return Result<EventResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Update Event
    public async Task<Result<EventResponseModel>> UpdateEvent(EventUpdateRequestModel request)
    {
        try
        {
            var _event = await _dbContext.TblEvents.FirstOrDefaultAsync(x => x.Id == request.Id && !x.DeleteFlag);
            if(_event == null)
            {
                return Result<EventResponseModel>.NotFound("Item Not Found!");
            }

            var eventStatus = request.Status.ToEventStatus();
            if (request.Name != null && !request.Name.IsValidEventName())
            {
                return Result<EventResponseModel>.ValidationError("Name is required and must not exceed 200 characters.");
            }

            if (request.Description != null && !request.Description.IsValidDescription())
            {
                return Result<EventResponseModel>.ValidationError("Description must not exceed 1000 characters.");
            }

            var eventStartAt = request.StartDate ?? _event.EventStartAt;
            var eventEndAt = request.EndDate ?? _event.EventEndAt;
            if ((request.StartDate.HasValue || request.EndDate.HasValue)
                && !eventEndAt.IsValidEventDate(eventStartAt))
            {
                return Result<EventResponseModel>.ValidationError("Event end date must be later than event start date.");
            }

            var votingStartAt = request.VotingStartAt ?? _event.VotingStartAt;
            var votingEndAt = request.VotingEndAt ?? _event.VotingEndAt;
            if ((request.VotingStartAt.HasValue || request.VotingEndAt.HasValue)
                && !votingEndAt.IsValidVotingDate(votingStartAt))
            {
                return Result<EventResponseModel>.ValidationError("Voting end date must be later than voting start date.");
            }

            if ((request.StartDate.HasValue
                    || request.EndDate.HasValue
                    || request.VotingStartAt.HasValue
                    || request.VotingEndAt.HasValue)
                && !votingStartAt.IsVotingInsideEvent(votingEndAt, eventStartAt, eventEndAt))
            {
                return Result<EventResponseModel>.ValidationError("Voting dates must be within the event date range.");
            }

            var resultShowStartAt = request.ResultShowStartAt ?? _event.ResultShowStartAt;
            var resultShowEndAt = request.ResultShowEndAt ?? _event.ResultShowEndAt;
            if ((request.ResultShowStartAt.HasValue || request.ResultShowEndAt.HasValue)
                && !resultShowEndAt.IsValidResultShowDate(resultShowStartAt))
            {
                return Result<EventResponseModel>.ValidationError("Result show end date must be later than result show start date.");
            }

            if (request.Status != null && !eventStatus.IsValidEventStatus())
            {
                return Result<EventResponseModel>.ValidationError("Status must be Upcoming, Active, Ended, or Published.");
            }

            if (request.Code != null && !request.Code.IsValidEventCode())
            {
                return Result<EventResponseModel>.ValidationError("Code is required and must not exceed 100 characters.");
            }

            if (request.Code != null
                && await _dbContext.TblEvents.AnyAsync(x => x.Code == request.Code && x.Id != request.Id))
            {
                return Result<EventResponseModel>.ValidationError("Event code already exists.");
            }

            if (request.Name != null)
            {
                _event.Name = request.Name;
            }

            if (request.Code != null)
            {
                _event.Code = request.Code;
            }

            if (request.Description != null)
            {
                _event.Description = request.Description;
            }

            if (request.StartDate.HasValue)
            {
                _event.EventStartAt = request.StartDate.Value;
            }

            if (request.EndDate.HasValue)
            {
                _event.EventEndAt = request.EndDate.Value;
            }

            if (request.VotingStartAt.HasValue)
            {
                _event.VotingStartAt = request.VotingStartAt.Value;
            }

            if (request.VotingEndAt.HasValue)
            {
                _event.VotingEndAt = request.VotingEndAt.Value;
            }

            if (request.ResultShowStartAt.HasValue)
            {
                _event.ResultShowStartAt = request.ResultShowStartAt.Value;
            }

            if (request.ResultShowEndAt.HasValue)
            {
                _event.ResultShowEndAt = request.ResultShowEndAt.Value;
            }

            if (request.Status != null)
            {
                _event.Status = eventStatus!.Value.ToString();
            }

            var hasUpdates = request.Name != null
                || request.Code != null
                || request.Description != null
                || request.StartDate.HasValue
                || request.EndDate.HasValue
                || request.VotingStartAt.HasValue
                || request.VotingEndAt.HasValue
                || request.ResultShowStartAt.HasValue
                || request.ResultShowEndAt.HasValue
                || request.Status != null;
            if (hasUpdates)
            {
                _event.ModifiedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }

            var resModel = new EventResponseModel
            {
                Id = _event.Id,
                Name = _event.Name,
                Code = _event.Code,
                Description = _event.Description,
                EventStartAt = _event.EventStartAt,
                EventEndAt = _event.EventEndAt,
                VotingStartAt = _event.VotingStartAt,
                VotingEndAt = _event.VotingEndAt,
                ResultShowStartAt = _event.ResultShowStartAt,
                ResultShowEndAt = _event.ResultShowEndAt,
                Status = _event.Status
            };

            return Result<EventResponseModel>.Success(resModel, "Event updated successfully.");
        }
        catch (Exception ex)
        {
            return Result<EventResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Delete Event
    public async Task<Result<EventResponseModel>> DeleteEvent(long id)
    {
        try
        {
            var _event = await _dbContext.TblEvents.FirstOrDefaultAsync(x => x.Id == id && !x.DeleteFlag);
            if (_event == null)
            {
                return Result<EventResponseModel>.NotFound("Item Not Found!");
            }
            _event.DeleteFlag = true;
            _event.IsActive = false;
            _event.ModifiedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return Result<EventResponseModel>.DeleteSuccess("Event deleted successfully.");
        }
        catch (Exception ex)
        {
            return Result<EventResponseModel>.SystemError(ex.Message);
        }
    }

    #endregion

}
