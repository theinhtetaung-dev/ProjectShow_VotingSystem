using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotingSystem.MVC.Models;

namespace VotingSystem.MVC.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class ProjectTypesController : Controller
{
    private const string ProjectTypeApiUrl = "api/ProjectType";
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProjectTypesController> _logger;

    public ProjectTypesController(IHttpClientFactory httpClientFactory, ILogger<ProjectTypesController> logger)
    {
        _httpClient = httpClientFactory.CreateClient("VotingSystemApi");
        _logger = logger;
    }

    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<ApiResult<PageResult<ProjectTypeResponseModel>>>($"{ProjectTypeApiUrl}?pageNumber={pageNumber}&pageSize={pageSize}");
            if (result?.IsSuccess != true)
            {
                TempData["ErrorMessage"] = result?.Message ?? "Unable to load projecttypes.";
                return View(new PageResult<ProjectTypeResponseModel>());
            }

            return View(result.Data ?? new PageResult<ProjectTypeResponseModel>());
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            TempData["ErrorMessage"] = "Unable to connect to the ProjectType API. Please try again.";
            return View(new PageResult<ProjectTypeResponseModel>());
        }
    }

    public IActionResult Create() => View(new ProjectTypeCreateRequestModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectTypeCreateRequestModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var response = await _httpClient.PostAsJsonAsync(ProjectTypeApiUrl, model);
            var result = await response.Content.ReadFromJsonAsync<ApiResult<ProjectTypeResponseModel>>();
            if (result?.IsSuccess != true)
            {
                ModelState.AddModelError(string.Empty, result?.Message ?? "Unable to create the projecttype.");
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message ?? "ProjectType created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            ModelState.AddModelError(string.Empty, "Unable to connect to the ProjectType API. Please try again.");
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var result = await GetById(id);
        if (result?.IsSuccess != true || result.Data is null)
            return RedirectToIndexWithError(result?.Message ?? "ProjectType not found.");

        var model = new ProjectTypeUpdateRequestModel
        {
            Id = result.Data.Id,
            Name = result.Data.Name,
            Code = result.Data.Code,
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProjectTypeUpdateRequestModel model)
    {
        if (id != model.Id)
            return BadRequest();
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var response = await _httpClient.PatchAsJsonAsync(ProjectTypeApiUrl, model);
            var result = await response.Content.ReadFromJsonAsync<ApiResult<ProjectTypeResponseModel>>();
            if (result?.IsSuccess != true)
            {
                ModelState.AddModelError(string.Empty, result?.Message ?? "Unable to update the projecttype.");
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message ?? "ProjectType updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            ModelState.AddModelError(string.Empty, "Unable to connect to the ProjectType API. Please try again.");
            return View(model);
        }
    }

    public async Task<IActionResult> Delete(int id)
    {
        var result = await GetById(id);
        if (result?.IsSuccess != true || result.Data is null)
            return RedirectToIndexWithError(result?.Message ?? "ProjectType not found.");
        return View(result.Data);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{ProjectTypeApiUrl}/{id}");
            var result = await response.Content.ReadFromJsonAsync<ApiResult<ProjectTypeResponseModel>>();
            TempData[result?.IsSuccess == true ? "SuccessMessage" : "ErrorMessage"] =
                result?.Message ?? "Unable to delete the projecttype.";
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            TempData["ErrorMessage"] = "Unable to connect to the ProjectType API. Please try again.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<ApiResult<ProjectTypeResponseModel>?> GetById(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ApiResult<ProjectTypeResponseModel>>($"{ProjectTypeApiUrl}/{id}");
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
        _logger.LogError(ex, "A ProjectType API request failed.");
    }
}

