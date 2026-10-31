using System.Net.Http.Headers;

namespace VotingSystem.MVC.Authentication;

public sealed class ApiAuthorizationHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Session
            .GetString(AuthSession.AccessToken);

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
