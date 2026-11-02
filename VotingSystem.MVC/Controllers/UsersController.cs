using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotingSystem.MVC.Models;

namespace VotingSystem.MVC.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class UsersController : Controller
{
    private const string UserApiUrl = "api/User";
    private readonly HttpClient _httpClient;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IHttpClientFactory httpClientFactory, ILogger<UsersController> logger)
    {
        _httpClient = httpClientFactory.CreateClient("VotingSystemApi");
        _logger = logger;
    }

    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<ApiResult<PageResult<UserResponseModel>>>($"{UserApiUrl}?pageNumber={pageNumber}&pageSize={pageSize}");
            if (result?.IsSuccess != true) TempData["ErrorMessage"] = result?.Message ?? "Unable to load users.";
            return View(result?.Data ?? new PageResult<UserResponseModel>());
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            TempData["ErrorMessage"] = "Unable to connect to the User API. Please try again.";
            return View(new PageResult<UserResponseModel>());
        }
    }

    public IActionResult Create() => View(new UserCreateRequestModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserCreateRequestModel model)
    {
        if (!ModelState.IsValid) return View(model);
        return await SaveUser(model, HttpMethod.Post, "create", View(model));
    }

    public async Task<IActionResult> Edit(long id)
    {
        var result = await GetUserById(id);
        if (result?.IsSuccess != true || result.Data is null) return RedirectToIndexWithError(result?.Message ?? "User not found.");
        return View(new UserUpdateRequestModel { Id = result.Data.Id, Email = result.Data.Email, Name = result.Data.Name, RoleId = result.Data.RoleId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, UserUpdateRequestModel model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);
        return await SaveUser(model, HttpMethod.Patch, "update", View(model));
    }

    public async Task<IActionResult> Delete(long id)
    {
        var result = await GetUserById(id);
        if (result?.IsSuccess != true || result.Data is null) return RedirectToIndexWithError(result?.Message ?? "User not found.");
        return View(result.Data);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{UserApiUrl}/{id}");
            var result = await response.Content.ReadFromJsonAsync<ApiResult<UserResponseModel>>();
            TempData[result?.IsSuccess == true ? "SuccessMessage" : "ErrorMessage"] = result?.Message ?? "Unable to delete the user.";
        }
        catch (Exception ex) when (IsApiException(ex)) { LogApiError(ex); TempData["ErrorMessage"] = "Unable to connect to the User API."; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignUser(AssignUserToEventRequestModel model)
    {
        return await RunUserOperation("assign-user-to-event", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveUser(RemoveUserFromEventRequestModel model)
    {
        return await RunUserOperation("remove-user-from-event", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignAdmin(AssignAdminToEventRequestModel model)
    {
        return await RunUserOperation("assign-admin-to-event", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveAdmin(RemoveAdminFromEventRequestModel model)
    {
        return await RunUserOperation("remove-admin-from-event", model);
    }

    private async Task<IActionResult> SaveUser(object model, HttpMethod method, string operation, IActionResult errorView)
    {
        try
        {
            using var request = new HttpRequestMessage(method, UserApiUrl) { Content = JsonContent.Create(model) };
            using var response = await _httpClient.SendAsync(request);
            var result = await response.Content.ReadFromJsonAsync<ApiResult<UserResponseModel>>();
            if (result?.IsSuccess != true) { ModelState.AddModelError(string.Empty, result?.Message ?? $"Unable to {operation} the user."); return errorView; }
            TempData["SuccessMessage"] = result.Message ?? $"User {operation}d successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (IsApiException(ex)) { LogApiError(ex); ModelState.AddModelError(string.Empty, "Unable to connect to the User API."); return errorView; }
    }

    private async Task<IActionResult> RunUserOperation(string route, object model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{UserApiUrl}/{route}", model);
            var result = await response.Content.ReadFromJsonAsync<ApiResult<JsonElement>>();
            TempData[result?.IsSuccess == true ? "SuccessMessage" : "ErrorMessage"] = result?.Message ?? "The user operation failed.";
        }
        catch (Exception ex) when (IsApiException(ex)) { LogApiError(ex); TempData["ErrorMessage"] = "Unable to connect to the User API."; }
        return RedirectToAction(nameof(Index));
    }

    private async Task<ApiResult<UserResponseModel>?> GetUserById(long id)
    {
        try { return await _httpClient.GetFromJsonAsync<ApiResult<UserResponseModel>>($"{UserApiUrl}/{id}"); }
        catch (Exception ex) when (IsApiException(ex)) { LogApiError(ex); return null; }
    }

    private IActionResult RedirectToIndexWithError(string message) { TempData["ErrorMessage"] = message; return RedirectToAction(nameof(Index)); }
    private static bool IsApiException(Exception ex) => ex is HttpRequestException or TaskCanceledException or JsonException;
    private void LogApiError(Exception ex) => _logger.LogError(ex, "A User API request failed.");
}
