using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using VotingSystem.DataAccess.Auditing;
using VotingSystem.DataAccess.AppDbContextModels;
using VotingSystem.Domain.Features.Auth;
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
            options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));
        var jwtOptions = builder.Configuration
            .GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>() ?? new JwtOptions();
        jwtOptions.Validate();

        builder.Services.Configure<JwtOptions>(
            builder.Configuration.GetSection(JwtOptions.SectionName));
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = jwtOptions.CreateSecurityKey(),
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
            });
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IPasswordHasher<TblUser>, PasswordHasher<TblUser>>();
        builder.Services.AddScoped<IAuthService, AuthService>();
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
