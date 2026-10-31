using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotingSystem.MVC.Authentication;
using VotingSystem.MVC.Models;

namespace VotingSystem.MVC.Controllers;

public class AccountController : Controller
{
    private const string AuthApiUrl = "api/Auth";
    private readonly HttpClient _httpClient;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IHttpClientFactory httpClientFactory, ILogger<AccountController> logger)
    {
        _httpClient = httpClientFactory.CreateClient("VotingSystemApi");
        _logger = logger;
    }

    [AllowAnonymous]
    public IActionResult Index(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Home");
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginRequestModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequestModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View("Index", model);
        }
        return await Authenticate($"{AuthApiUrl}/login", model, returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh(RefreshTokenRequestModel model) => await Authenticate($"{AuthApiUrl}/refresh", model);

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = HttpContext.Session.GetString(AuthSession.RefreshToken);
        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            await PostLogout($"{AuthApiUrl}/logout", new LogoutRequestModel { RefreshToken = refreshToken });
        }
        HttpContext.Session.Clear();
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> LogoutAll()
    {
        var token = HttpContext.Session.GetString("AccessToken");
        if (!string.IsNullOrWhiteSpace(token)) _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        await PostLogout($"{AuthApiUrl}/logout-all", null);
        HttpContext.Session.Clear();
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Index));
    }

    [AllowAnonymous]
    public IActionResult AccessDenied() => View();

    private async Task<IActionResult> Authenticate(string url, object model, string? returnUrl = null)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, model);
            var result = await response.Content.ReadFromJsonAsync<ApiResult<AuthResponseModel>>();
            if (result?.IsSuccess != true || result.Data is null)
            {
                ModelState.AddModelError(string.Empty, result?.Message ?? "Authentication failed.");
                return View("Index", new LoginRequestModel());
            }
            SessionAuthenticationFilter.SaveTokens(HttpContext.Session, result.Data);
            await SignIn(result.Data);
            TempData["SuccessMessage"] = result.Message ?? "Authentication successful.";

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            TempData["ErrorMessage"] = "Unable to connect to the Auth API. Please try again.";
        }
        ViewData["ReturnUrl"] = returnUrl;
        return View("Index", model);
    }

    private async Task SignIn(AuthResponseModel authentication)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, authentication.UserId.ToString()),
            new(ClaimTypes.Name, authentication.Name),
            new(ClaimTypes.Email, authentication.Email),
            new(ClaimTypes.Role, authentication.Role)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var properties = new AuthenticationProperties
        {
            IsPersistent = false,
            ExpiresUtc = new DateTimeOffset(authentication.RefreshTokenExpiresAt.ToUniversalTime())
        };
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            properties);
    }

    private async Task PostLogout(string url, object? model)
    {
        try
        {
            var response = model is null ? await _httpClient.PostAsync(url, null) : await _httpClient.PostAsJsonAsync(url, model);
            var result = await response.Content.ReadFromJsonAsync<ApiResult<JsonElement>>();
            TempData[result?.IsSuccess == true ? "SuccessMessage" : "ErrorMessage"] = result?.Message ?? "Logout failed.";
        }
        catch (Exception ex) when (IsApiException(ex)) { LogApiError(ex); TempData["ErrorMessage"] = "Unable to connect to the Auth API."; }
    }

    private static bool IsApiException(Exception ex) => ex is HttpRequestException or TaskCanceledException or JsonException;
    private void LogApiError(Exception ex) => _logger.LogError(ex, "An Auth API request failed.");
}
