using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using VotingSystem.DataAccess.Auditing;
using VotingSystem.Domain.Features.AuditLog;

namespace VotingSystem.Domain.Features;

public static class FeaturesManager
{
    public static void AddDomain(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IAuditActorProvider, HttpContextAuditActorProvider>();
        builder.Services.AddScoped<IAuditLogService, AuditLogService>();
    }
}
