using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using VotingSystem.MVC.Models;

namespace VotingSystem.MVC.Controllers;

public class VotingsController : Controller
{
    private const string VotingApiUrl = "api/Voting";
    private readonly HttpClient _httpClient;
    private readonly ILogger<VotingsController> _logger;

    public VotingsController(IHttpClientFactory httpClientFactory, ILogger<VotingsController> logger)
    {
        _httpClient = httpClientFactory.CreateClient("VotingSystemApi");
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<ApiResult<List<VotingResponseModel>>>(VotingApiUrl);
            if (result?.IsSuccess != true)
                TempData["ErrorMessage"] = result?.Message ?? "Unable to load voting records.";
            return View(result?.Data ?? []);
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            TempData["ErrorMessage"] = "Unable to connect to the Voting API. Please try again.";
            return View(new List<VotingResponseModel>());
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Vote(VoteRequestModel model)
    {
        await SendOperation(HttpMethod.Post, $"{VotingApiUrl}/vote", model, "Unable to cast the vote.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateLimit(VotingLimitUpdateRequestModel model)
    {
        await SendOperation(HttpMethod.Patch, $"{VotingApiUrl}/limit", model, "Unable to update the voting limit.");
        return RedirectToAction(nameof(Index));
    }

    private async Task SendOperation(HttpMethod method, string url, object model, string defaultError)
    {
        try
        {
            using var request = new HttpRequestMessage(method, url) { Content = JsonContent.Create(model) };
            using var response = await _httpClient.SendAsync(request);
            var result = await response.Content.ReadFromJsonAsync<ApiResult<JsonElement>>();
            TempData[result?.IsSuccess == true ? "SuccessMessage" : "ErrorMessage"] = result?.Message ?? defaultError;
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            TempData["ErrorMessage"] = "Unable to connect to the Voting API. Please try again.";
        }
    }

    private static bool IsApiException(Exception ex) =>
        ex is HttpRequestException or TaskCanceledException or JsonException;

    private void LogApiError(Exception ex) => _logger.LogError(ex, "A Voting API request failed.");
}
