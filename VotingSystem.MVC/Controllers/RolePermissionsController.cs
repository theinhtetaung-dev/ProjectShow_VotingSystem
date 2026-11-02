using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotingSystem.MVC.Models;

namespace VotingSystem.MVC.Controllers;

[Authorize(Roles = "SuperAdmin")]
public class RolePermissionsController : Controller
{
    private const string RolePermissionApiUrl = "api/RolePermission";
    private readonly HttpClient _httpClient;
    private readonly ILogger<RolePermissionsController> _logger;

    public RolePermissionsController(IHttpClientFactory httpClientFactory, ILogger<RolePermissionsController> logger)
    {
        _httpClient = httpClientFactory.CreateClient("VotingSystemApi");
        _logger = logger;
    }

    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<ApiResult<PageResult<RolePermissionResponseModel>>>($"{RolePermissionApiUrl}?pageNumber={pageNumber}&pageSize={pageSize}");
            if (result?.IsSuccess != true)
                TempData["ErrorMessage"] = result?.Message ?? "Unable to load role permissions.";
            return View(result?.Data ?? new PageResult<RolePermissionResponseModel>());
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            TempData["ErrorMessage"] = "Unable to connect to the Role Permission API. Please try again.";
            return View(new PageResult<RolePermissionResponseModel>());
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Assign(AssignPermissionToRoleRequestModel model)
    {
        await PostOperation($"{RolePermissionApiUrl}/assign", model, "assign the permission");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(RemovePermissionFromRoleRequestModel model)
    {
        await PostOperation($"{RolePermissionApiUrl}/remove", model, "remove the permission");
        return RedirectToAction(nameof(Index));
    }

    private async Task PostOperation(string url, object model, string operation)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, model);
            var result = await response.Content.ReadFromJsonAsync<ApiResult<RolePermissionResponseModel>>();
            TempData[result?.IsSuccess == true ? "SuccessMessage" : "ErrorMessage"] =
                result?.Message ?? $"Unable to {operation}.";
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            TempData["ErrorMessage"] = "Unable to connect to the Role Permission API. Please try again.";
        }
    }

    private static bool IsApiException(Exception ex) =>
        ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException;

    private void LogApiError(Exception ex) =>
        _logger.LogError(ex, "A Role Permission API request failed.");
}
