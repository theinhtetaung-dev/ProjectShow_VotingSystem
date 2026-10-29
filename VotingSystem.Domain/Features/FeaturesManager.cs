using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VotingSystem.DataAccess.Auditing;
using VotingSystem.DataAccess.AppDbContextModels;
using VotingSystem.Domain.Features.AuditLog;
using VotingSystem.Domain.Features.Event;

namespace VotingSystem.Domain.Features;

public static class FeaturesManager
{
    public static void AddDomain(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IAuditActorProvider, HttpContextAuditActorProvider>();
        builder.Services.AddScoped<IAuditLogService, AuditLogService>();
        builder.Services.AddScoped<IEventService, EventService>();
    }
}
