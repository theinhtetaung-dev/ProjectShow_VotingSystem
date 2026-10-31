using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using VotingSystem.MVC.Models;

namespace VotingSystem.MVC.Controllers;

public class RolesController : Controller
{
    private const string RoleApiUrl = "api/Role";
    private readonly HttpClient _httpClient;
    private readonly ILogger<RolesController> _logger;

    public RolesController(IHttpClientFactory httpClientFactory, ILogger<RolesController> logger)
    {
        _httpClient = httpClientFactory.CreateClient("VotingSystemApi");
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<ApiResult<List<RoleResponseModel>>>(RoleApiUrl);
            if (result?.IsSuccess != true)
            {
                TempData["ErrorMessage"] = result?.Message ?? "Unable to load roles.";
                return View(new List<RoleResponseModel>());
            }

            return View(result.Data ?? new List<RoleResponseModel>());
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            TempData["ErrorMessage"] = "Unable to connect to the Role API. Please try again.";
            return View(new List<RoleResponseModel>());
        }
    }

    public IActionResult Create() => View(new RoleCreateRequestModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RoleCreateRequestModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var response = await _httpClient.PostAsJsonAsync(RoleApiUrl, model);
            var result = await response.Content.ReadFromJsonAsync<ApiResult<RoleResponseModel>>();
            if (result?.IsSuccess != true)
            {
                ModelState.AddModelError(string.Empty, result?.Message ?? "Unable to create the role.");
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message ?? "Role created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            ModelState.AddModelError(string.Empty, "Unable to connect to the Role API. Please try again.");
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var result = await GetById(id);
        if (result?.IsSuccess != true || result.Data is null)
            return RedirectToIndexWithError(result?.Message ?? "Role not found.");

        var model = new RoleUpdateRequestModel
        {
            Id = result.Data.Id,
            Name = result.Data.Name,
            Description = result.Data.Description,
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RoleUpdateRequestModel model)
    {
        if (id != model.Id)
            return BadRequest();
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var response = await _httpClient.PatchAsJsonAsync(RoleApiUrl, model);
            var result = await response.Content.ReadFromJsonAsync<ApiResult<RoleResponseModel>>();
            if (result?.IsSuccess != true)
            {
                ModelState.AddModelError(string.Empty, result?.Message ?? "Unable to update the role.");
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message ?? "Role updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            ModelState.AddModelError(string.Empty, "Unable to connect to the Role API. Please try again.");
            return View(model);
        }
    }

    public async Task<IActionResult> Delete(int id)
    {
        var result = await GetById(id);
        if (result?.IsSuccess != true || result.Data is null)
            return RedirectToIndexWithError(result?.Message ?? "Role not found.");
        return View(result.Data);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{RoleApiUrl}/{id}");
            var result = await response.Content.ReadFromJsonAsync<ApiResult<RoleResponseModel>>();
            TempData[result?.IsSuccess == true ? "SuccessMessage" : "ErrorMessage"] =
                result?.Message ?? "Unable to delete the role.";
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            TempData["ErrorMessage"] = "Unable to connect to the Role API. Please try again.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<ApiResult<RoleResponseModel>?> GetById(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ApiResult<RoleResponseModel>>($"{RoleApiUrl}/{id}");
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
        _logger.LogError(ex, "A Role API request failed.");
    }
}

