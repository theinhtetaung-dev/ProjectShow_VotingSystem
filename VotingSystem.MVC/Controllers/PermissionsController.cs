using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotingSystem.MVC.Models;

namespace VotingSystem.MVC.Controllers;

[Authorize(Roles = "SuperAdmin")]
public class PermissionsController : Controller
{
    private const string PermissionApiUrl = "api/Permission";
    private readonly HttpClient _httpClient;
    private readonly ILogger<PermissionsController> _logger;

    public PermissionsController(IHttpClientFactory httpClientFactory, ILogger<PermissionsController> logger)
    {
        _httpClient = httpClientFactory.CreateClient("VotingSystemApi");
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<ApiResult<List<PermissionResponseModel>>>(PermissionApiUrl);
            if (result?.IsSuccess != true)
            {
                TempData["ErrorMessage"] = result?.Message ?? "Unable to load permissions.";
                return View(new List<PermissionResponseModel>());
            }

            return View(result.Data ?? new List<PermissionResponseModel>());
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            TempData["ErrorMessage"] = "Unable to connect to the Permission API. Please try again.";
            return View(new List<PermissionResponseModel>());
        }
    }

    public IActionResult Create() => View(new PermissionCreateRequestModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PermissionCreateRequestModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var response = await _httpClient.PostAsJsonAsync(PermissionApiUrl, model);
            var result = await response.Content.ReadFromJsonAsync<ApiResult<PermissionResponseModel>>();
            if (result?.IsSuccess != true)
            {
                ModelState.AddModelError(string.Empty, result?.Message ?? "Unable to create the permission.");
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message ?? "Permission created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            ModelState.AddModelError(string.Empty, "Unable to connect to the Permission API. Please try again.");
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var result = await GetById(id);
        if (result?.IsSuccess != true || result.Data is null)
            return RedirectToIndexWithError(result?.Message ?? "Permission not found.");

        var model = new PermissionUpdateRequestModel
        {
            Id = result.Data.Id,
            Name = result.Data.Name,
            Description = result.Data.Description,
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PermissionUpdateRequestModel model)
    {
        if (id != model.Id)
            return BadRequest();
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var response = await _httpClient.PatchAsJsonAsync(PermissionApiUrl, model);
            var result = await response.Content.ReadFromJsonAsync<ApiResult<PermissionResponseModel>>();
            if (result?.IsSuccess != true)
            {
                ModelState.AddModelError(string.Empty, result?.Message ?? "Unable to update the permission.");
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message ?? "Permission updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            ModelState.AddModelError(string.Empty, "Unable to connect to the Permission API. Please try again.");
            return View(model);
        }
    }

    public async Task<IActionResult> Delete(int id)
    {
        var result = await GetById(id);
        if (result?.IsSuccess != true || result.Data is null)
            return RedirectToIndexWithError(result?.Message ?? "Permission not found.");
        return View(result.Data);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{PermissionApiUrl}/{id}");
            var result = await response.Content.ReadFromJsonAsync<ApiResult<PermissionResponseModel>>();
            TempData[result?.IsSuccess == true ? "SuccessMessage" : "ErrorMessage"] =
                result?.Message ?? "Unable to delete the permission.";
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            TempData["ErrorMessage"] = "Unable to connect to the Permission API. Please try again.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<ApiResult<PermissionResponseModel>?> GetById(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ApiResult<PermissionResponseModel>>($"{PermissionApiUrl}/{id}");
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
        _logger.LogError(ex, "A Permission API request failed.");
    }
}

