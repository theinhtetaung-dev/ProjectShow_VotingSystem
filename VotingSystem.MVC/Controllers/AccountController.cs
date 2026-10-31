using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
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

    public IActionResult Index() => View(new LoginRequestModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequestModel model)
    {
        if (!ModelState.IsValid) return View("Index", model);
        return await Authenticate($"{AuthApiUrl}/login", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Refresh(RefreshTokenRequestModel model) => await Authenticate($"{AuthApiUrl}/refresh", model);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(LogoutRequestModel model)
    {
        await PostLogout($"{AuthApiUrl}/logout", model);
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogoutAll()
    {
        var token = HttpContext.Session.GetString("AccessToken");
        if (!string.IsNullOrWhiteSpace(token)) _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        await PostLogout($"{AuthApiUrl}/logout-all", null);
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Index));
    }

    private async Task<IActionResult> Authenticate(string url, object model)
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
            HttpContext.Session.SetString("AccessToken", result.Data.AccessToken);
            HttpContext.Session.SetString("RefreshToken", result.Data.RefreshToken);
            TempData["SuccessMessage"] = result.Message ?? "Authentication successful.";
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            TempData["ErrorMessage"] = "Unable to connect to the Auth API. Please try again.";
        }
        return RedirectToAction(nameof(Index));
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
