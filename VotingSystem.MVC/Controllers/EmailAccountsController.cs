using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotingSystem.MVC.Models;

namespace VotingSystem.MVC.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class EmailAccountsController : Controller
{
    private const string EmailAccountApiUrl = "api/EmailAccount";
    private readonly HttpClient _httpClient;
    private readonly ILogger<EmailAccountsController> _logger;

    public EmailAccountsController(
        IHttpClientFactory httpClientFactory,
        ILogger<EmailAccountsController> logger)
    {
        _httpClient = httpClientFactory.CreateClient("VotingSystemApi");
        _logger = logger;
    }

    public async Task<IActionResult> Index(
        int pageNumber = 1,
        int pageSize = 10,
        string? status = null,
        CancellationToken cancellationToken = default)
    {
        ViewBag.SelectedStatus = status;

        try
        {
            var query = $"{EmailAccountApiUrl}?pageNumber={pageNumber}&pageSize={pageSize}";
            if (string.Equals(status, "active", StringComparison.OrdinalIgnoreCase))
            {
                query += "&isActive=true";
            }
            else if (string.Equals(status, "disabled", StringComparison.OrdinalIgnoreCase))
            {
                query += "&isActive=false";
            }

            var result = await _httpClient.GetFromJsonAsync<ApiResult<PageResult<EmailAccountResponseModel>>>(
                query,
                cancellationToken);

            if (result?.IsSuccess != true)
            {
                TempData["ErrorMessage"] = result?.Message ?? "Unable to load email accounts.";
                return View(new PageResult<EmailAccountResponseModel>());
            }

            return View(result.Data ?? new PageResult<EmailAccountResponseModel>());
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            TempData["ErrorMessage"] = "Unable to connect to the Email Account API. Please try again.";
            return View(new PageResult<EmailAccountResponseModel>());
        }
    }

    public async Task<IActionResult> Details(long id, CancellationToken cancellationToken = default)
    {
        var result = await GetByIdAsync(id, cancellationToken);
        if (result?.IsSuccess != true || result.Data is null)
        {
            return RedirectToIndexWithError(result?.Message ?? "Email account not found.");
        }

        return View(result.Data);
    }

    public IActionResult Create()
    {
        return View(new EmailAccountCreateRequestModel
        {
            PriorityOrder = 1,
            IsActive = true
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        EmailAccountCreateRequestModel model,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var response = await _httpClient.PostAsJsonAsync(EmailAccountApiUrl, model, cancellationToken);
            var result = await ReadApiResultAsync<EmailAccountResponseModel>(response, cancellationToken);

            if (!response.IsSuccessStatusCode || result?.IsSuccess != true)
            {
                ModelState.AddModelError(string.Empty, result?.Message ?? "Unable to create the email account.");
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message ?? "Email account created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            ModelState.AddModelError(string.Empty, "Unable to connect to the Email Account API. Please try again.");
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(long id, CancellationToken cancellationToken = default)
    {
        var result = await GetByIdAsync(id, cancellationToken);
        if (result?.IsSuccess != true || result.Data is null)
        {
            return RedirectToIndexWithError(result?.Message ?? "Email account not found.");
        }

        var model = new EmailAccountUpdateRequestModel
        {
            Id = result.Data.Id,
            EmailAddress = result.Data.EmailAddress,
            DisplayName = result.Data.DisplayName,
            PriorityOrder = result.Data.PriorityOrder,
            IsActive = result.Data.IsActive
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        long id,
        EmailAccountUpdateRequestModel model,
        CancellationToken cancellationToken = default)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var payload = new
            {
                model.Id,
                model.EmailAddress,
                DisplayName = model.DisplayName ?? string.Empty,
                AppPassword = string.IsNullOrWhiteSpace(model.AppPassword) ? null : model.AppPassword,
                model.PriorityOrder,
                model.IsActive
            };

            var response = await _httpClient.PatchAsJsonAsync(EmailAccountApiUrl, payload, cancellationToken);
            var result = await ReadApiResultAsync<EmailAccountResponseModel>(response, cancellationToken);

            if (!response.IsSuccessStatusCode || result?.IsSuccess != true)
            {
                ModelState.AddModelError(string.Empty, result?.Message ?? "Unable to update the email account.");
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message ?? "Email account updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            ModelState.AddModelError(string.Empty, "Unable to connect to the Email Account API. Please try again.");
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(
        long id,
        bool isActive,
        string? returnTo = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PatchAsJsonAsync(
                $"{EmailAccountApiUrl}/{id}/status",
                new EmailAccountStatusUpdateRequestModel { IsActive = isActive },
                cancellationToken);
            var result = await ReadApiResultAsync<EmailAccountResponseModel>(response, cancellationToken);

            TempData[response.IsSuccessStatusCode && result?.IsSuccess == true ? "SuccessMessage" : "ErrorMessage"] =
                result?.Message ?? "Unable to update email account status.";
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            TempData["ErrorMessage"] = "Unable to connect to the Email Account API. Please try again.";
        }

        if (string.Equals(returnTo, nameof(Details), StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction(nameof(Details), new { id });
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{EmailAccountApiUrl}/{id}", cancellationToken);
            var result = await ReadApiResultAsync<EmailAccountResponseModel>(response, cancellationToken);

            TempData[response.IsSuccessStatusCode && result?.IsSuccess == true ? "SuccessMessage" : "ErrorMessage"] =
                result?.Message ?? "Unable to delete the email account.";
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            TempData["ErrorMessage"] = "Unable to connect to the Email Account API. Please try again.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<ApiResult<EmailAccountResponseModel>?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken)
    {
        try
        {
            using var response = await _httpClient.GetAsync($"{EmailAccountApiUrl}/{id}", cancellationToken);
            return await ReadApiResultAsync<EmailAccountResponseModel>(response, cancellationToken);
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            return null;
        }
    }

    private static async Task<ApiResult<T>?> ReadApiResultAsync<T>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.Content.Headers.ContentLength == 0)
        {
            return null;
        }

        try
        {
            return await response.Content.ReadFromJsonAsync<ApiResult<T>>(cancellationToken);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private IActionResult RedirectToIndexWithError(string message)
    {
        TempData["ErrorMessage"] = message;
        return RedirectToAction(nameof(Index));
    }

    private static bool IsApiException(Exception ex) =>
        ex is HttpRequestException or TaskCanceledException or JsonException;

    private void LogApiError(Exception ex)
    {
        _logger.LogError(ex, "An Email Account API request failed.");
    }
}
