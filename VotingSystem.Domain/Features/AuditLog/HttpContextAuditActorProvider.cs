using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using VotingSystem.DataAccess.Auditing;

namespace VotingSystem.Domain.Features.AuditLog;

public class HttpContextAuditActorProvider : IAuditActorProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextAuditActorProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public long? UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            var value = user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user.FindFirstValue("sub");
            return long.TryParse(value, out var userId) ? userId : null;
        }
    }
}
