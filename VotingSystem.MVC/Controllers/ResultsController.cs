using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using VotingSystem.MVC.Models;

namespace VotingSystem.MVC.Controllers;

public class ResultsController : Controller
{
    private const string ResultApiUrl = "api/Result";
    private readonly HttpClient _httpClient;
    private readonly ILogger<ResultsController> _logger;

    public ResultsController(IHttpClientFactory httpClientFactory, ILogger<ResultsController> logger)
    {
        _httpClient = httpClientFactory.CreateClient("VotingSystemApi");
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<ApiResult<List<ResultResponseModel>>>(ResultApiUrl);
            if (result?.IsSuccess != true) TempData["ErrorMessage"] = result?.Message ?? "Unable to load results.";
            return View(result?.Data ?? []);
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            TempData["ErrorMessage"] = "Unable to connect to the Result API. Please try again.";
            return View(new List<ResultResponseModel>());
        }
    }

    public IActionResult Create() => View(new ResultCreateRequestModel { Version = 1 });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ResultCreateRequestModel model)
    {
        if (!ModelState.IsValid) return View(model);
        return await Save(model, HttpMethod.Post, "create", View(model));
    }

    public async Task<IActionResult> Edit(long id)
    {
        var result = await GetById(id);
        if (result?.IsSuccess != true || result.Data is null) return RedirectToIndexWithError(result?.Message ?? "Result not found.");
        var model = new ResultUpdateRequestModel
        {
            Id = result.Data.Id, EventId = result.Data.EventId, ProjectId = result.Data.ProjectId,
            Status = result.Data.Status, Version = result.Data.Version, ReviewedAt = result.Data.ReviewedAt,
            ReviewedBy = result.Data.ReviewedBy, PublishedAt = result.Data.PublishedAt, PublishedBy = result.Data.PublishedBy
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, ResultUpdateRequestModel model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);
        return await Save(model, HttpMethod.Patch, "update", View(model));
    }

    private async Task<IActionResult> Save(object model, HttpMethod method, string operation, IActionResult errorView)
    {
        try
        {
            using var request = new HttpRequestMessage(method, ResultApiUrl) { Content = JsonContent.Create(model) };
            using var response = await _httpClient.SendAsync(request);
            var result = await response.Content.ReadFromJsonAsync<ApiResult<ResultResponseModel>>();
            if (result?.IsSuccess != true)
            {
                ModelState.AddModelError(string.Empty, result?.Message ?? $"Unable to {operation} the result.");
                return errorView;
            }
            TempData["SuccessMessage"] = result.Message ?? $"Result {operation}d successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            ModelState.AddModelError(string.Empty, "Unable to connect to the Result API. Please try again.");
            return errorView;
        }
    }

    private async Task<ApiResult<ResultResponseModel>?> GetById(long id)
    {
        try { return await _httpClient.GetFromJsonAsync<ApiResult<ResultResponseModel>>($"{ResultApiUrl}/{id}"); }
        catch (Exception ex) when (IsApiException(ex)) { LogApiError(ex); return null; }
    }

    private IActionResult RedirectToIndexWithError(string message) { TempData["ErrorMessage"] = message; return RedirectToAction(nameof(Index)); }
    private static bool IsApiException(Exception ex) => ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException;
    private void LogApiError(Exception ex) => _logger.LogError(ex, "A Result API request failed.");
}
