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

    public async Task<Result<List<EventResponseModel>>> GetEventlist()
    {
        try
        {
            var events = await _dbContext.TblEvents.ToListAsync();
            var resModel = new List<EventResponseModel>();

            foreach (var ev in events)
            {
                resModel.Add(new EventResponseModel
                {
                    Id = ev.Id,
                    Name = ev.Name,
                    Description = ev.Description,
                    EventStartAt = ev.EventStartAt,
                    EventEndAt = ev.EventEndAt,
                    Status = ev.IsActive ? "Active" : "Inactive"
                });
            }
            string message = events.Count() == 0 ? "No events found." : "Success";

            return Result<List<EventResponseModel>>.Success(resModel, message);
        }
        catch (Exception ex)
        {
            return Result<List<EventResponseModel>>.SystemError(ex.Message);
        }
    }

    public async Task<Result<EventResponseModel>> GetEventByID(int id)
    {
        try
        {
            var _event = await _dbContext.TblEvents.FirstOrDefaultAsync(x => x.Id == id);
            if(_event == null)
            {
                return Result<EventResponseModel>.NotFound("Item Not Found!");
            }

            var resModel = new EventResponseModel()
            {
                Id = _event.Id,
                Name = _event.Name,
                Description = _event.Description,
                EventStartAt = _event.EventStartAt,
                EventEndAt = _event.EventEndAt,
                Status = _event.IsActive ? "Active" : "Inactive"
            };
            return Result<EventResponseModel>.Success(resModel, "Success");
        }

        catch (Exception ex)
        {
            return Result<EventResponseModel>.SystemError(ex.Message);
        }
    }
        


}
