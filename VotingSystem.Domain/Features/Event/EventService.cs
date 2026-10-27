using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VotingSystem.DataAccess.AppDbContextModels;

namespace VotingSystem.Domain.Features.Event;

public class EventService
{
    private readonly AppDbContext _dbContext;

    public EventService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }



}
