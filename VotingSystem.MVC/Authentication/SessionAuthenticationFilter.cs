using System.Globalization;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VotingSystem.MVC.Models;

namespace VotingSystem.MVC.Authentication;

// This filter keeps the API access token in sync with the MVC login cookie.
// Role checks still belong to ASP.NET Core's [Authorize] attribute.
public sealed class SessionAuthenticationFilter : IAsyncActionFilter
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SessionAuthenticationFilter> _logger;

    public SessionAuthenticationFilter(
        IHttpClientFactory httpClientFactory,
        ILogger<SessionAuthenticationFilter> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        if (IsAnonymousAction(context) || context.HttpContext.User.Identity?.IsAuthenticated != true)
        {
            await next();
            return;
        }

        if (AccessTokenIsValid(context.HttpContext.Session))
        {
            await next();
            return;
        }

        if (await TryRefreshToken(context.HttpContext))
        {
            await next();
            return;
        }

        await SignOut(context.HttpContext);
        context.Result = new RedirectToActionResult("Index", "Account", new { returnUrl = context.HttpContext.Request.Path });
    }

    private static bool IsAnonymousAction(ActionExecutingContext context)
    {
        return context.ActionDescriptor.EndpointMetadata
            .OfType<IAllowAnonymous>()
            .Any();
    }

    private static bool AccessTokenIsValid(ISession session)
    {
        var value = session.GetString(AuthSession.AccessTokenExpiresAt);
        return DateTime.TryParse(value, CultureInfo.InvariantCulture,
                   DateTimeStyles.RoundtripKind, out var expiresAt)
               && expiresAt > DateTime.UtcNow.AddSeconds(30)
               && !string.IsNullOrWhiteSpace(session.GetString(AuthSession.AccessToken));
    }

    private async Task<bool> TryRefreshToken(HttpContext context)
    {
        var refreshToken = context.Session.GetString(AuthSession.RefreshToken);
        if (string.IsNullOrWhiteSpace(refreshToken)) return false;

        try
        {
            var client = _httpClientFactory.CreateClient("VotingSystemAuthApi");
            using var response = await client.PostAsJsonAsync(
                "api/Auth/refresh",
                new RefreshTokenRequestModel { RefreshToken = refreshToken });
            if (!response.IsSuccessStatusCode) return false;

            var result = await response.Content.ReadFromJsonAsync<ApiResult<AuthResponseModel>>();
            if (result?.IsSuccess != true || result.Data is null) return false;

            SaveTokens(context.Session, result.Data);
            return true;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
        {
            _logger.LogWarning(ex, "The API token could not be refreshed.");
            return false;
        }
    }

    public static void SaveTokens(ISession session, AuthResponseModel authentication)
    {
        session.SetString(AuthSession.AccessToken, authentication.AccessToken);
        session.SetString(AuthSession.AccessTokenExpiresAt,
            authentication.AccessTokenExpiresAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture));
        session.SetString(AuthSession.RefreshToken, authentication.RefreshToken);
    }

    private static async Task SignOut(HttpContext context)
    {
        context.Session.Clear();
        await context.SignOutAsync();
    }
}
