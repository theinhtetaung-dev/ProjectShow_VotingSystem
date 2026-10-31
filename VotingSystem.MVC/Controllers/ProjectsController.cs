using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotingSystem.MVC.Models;

namespace VotingSystem.MVC.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class ProjectsController : Controller
{
    private const string ProjectApiUrl = "api/Project";
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(IHttpClientFactory httpClientFactory, ILogger<ProjectsController> logger)
    {
        _httpClient = httpClientFactory.CreateClient("VotingSystemApi");
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<ApiResult<List<ProjectResponseModel>>>(ProjectApiUrl);
            if (result?.IsSuccess != true)
            {
                TempData["ErrorMessage"] = result?.Message ?? "Unable to load projects.";
                return View(new List<ProjectResponseModel>());
            }

            return View(result.Data ?? new List<ProjectResponseModel>());
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            TempData["ErrorMessage"] = "Unable to connect to the Project API. Please try again.";
            return View(new List<ProjectResponseModel>());
        }
    }

    public IActionResult Create() => View(new ProjectCreateRequestModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectCreateRequestModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var response = await _httpClient.PostAsJsonAsync(ProjectApiUrl, model);
            var result = await response.Content.ReadFromJsonAsync<ApiResult<ProjectResponseModel>>();
            if (result?.IsSuccess != true)
            {
                ModelState.AddModelError(string.Empty, result?.Message ?? "Unable to create the project.");
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message ?? "Project created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            ModelState.AddModelError(string.Empty, "Unable to connect to the Project API. Please try again.");
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(long id)
    {
        var result = await GetById(id);
        if (result?.IsSuccess != true || result.Data is null)
            return RedirectToIndexWithError(result?.Message ?? "Project not found.");

        var model = new ProjectUpdateRequestModel
        {
            Id = result.Data.Id,
            EventId = result.Data.EventId,
            ProjectTypeId = result.Data.ProjectTypeId,
            Name = result.Data.Name,
            Description = result.Data.Description,
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, ProjectUpdateRequestModel model)
    {
        if (id != model.Id)
            return BadRequest();
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var response = await _httpClient.PatchAsJsonAsync(ProjectApiUrl, model);
            var result = await response.Content.ReadFromJsonAsync<ApiResult<ProjectResponseModel>>();
            if (result?.IsSuccess != true)
            {
                ModelState.AddModelError(string.Empty, result?.Message ?? "Unable to update the project.");
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message ?? "Project updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            ModelState.AddModelError(string.Empty, "Unable to connect to the Project API. Please try again.");
            return View(model);
        }
    }

    public async Task<IActionResult> Delete(long id)
    {
        var result = await GetById(id);
        if (result?.IsSuccess != true || result.Data is null)
            return RedirectToIndexWithError(result?.Message ?? "Project not found.");
        return View(result.Data);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{ProjectApiUrl}/{id}");
            var result = await response.Content.ReadFromJsonAsync<ApiResult<ProjectResponseModel>>();
            TempData[result?.IsSuccess == true ? "SuccessMessage" : "ErrorMessage"] =
                result?.Message ?? "Unable to delete the project.";
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            TempData["ErrorMessage"] = "Unable to connect to the Project API. Please try again.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<ApiResult<ProjectResponseModel>?> GetById(long id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ApiResult<ProjectResponseModel>>($"{ProjectApiUrl}/{id}");
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            return null;
        }
    }

    private IActionResult RedirectToIndexWithError(string message)
    {
        TempData["ErrorMessage"] = message;
        return RedirectToAction(nameof(Index));
    }

    private static bool IsApiException(Exception ex) =>
        ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException;

    private void LogApiError(Exception ex)
    {
        _logger.LogError(ex, "A Project API request failed.");
    }
}

