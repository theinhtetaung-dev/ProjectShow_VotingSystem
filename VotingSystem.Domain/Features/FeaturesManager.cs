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
using VotingSystem.Domain.Features.Permission;
using VotingSystem.Domain.Features.Project;
using VotingSystem.Domain.Features.ProjectType;
using VotingSystem.Domain.Features.Result;
using VotingSystem.Domain.Features.Role;
using VotingSystem.Domain.Features.RolePermission;
using VotingSystem.Domain.Features.User;
using VotingSystem.Domain.Features.Voting;

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
        builder.Services.AddScoped<IProjectService, ProjectService>();
        builder.Services.AddScoped<IProjectTypeService, ProjectTypeService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IVotingService, VotingService>();
        builder.Services.AddScoped<IResultService, ResultService>();
        builder.Services.AddScoped<IRoleService, RoleService>();
        builder.Services.AddScoped<IPermissionService, PermissionService>();
        builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
    }
}
