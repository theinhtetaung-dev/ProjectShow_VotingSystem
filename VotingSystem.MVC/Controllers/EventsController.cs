using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotingSystem.MVC.Models;

namespace VotingSystem.MVC.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class EventsController : Controller
{
    private const string EventApiUrl = "api/Event";
    private readonly HttpClient _httpClient;
    private readonly ILogger<EventsController> _logger;

    public EventsController(IHttpClientFactory httpClientFactory, ILogger<EventsController> logger)
    {
        _httpClient = httpClientFactory.CreateClient("VotingSystemApi");
        _logger = logger;
    }

    [ActionName("Index")]
    public async Task<IActionResult> EventIndex(int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<ApiResult<PageResult<EventResponseModel>>>($"{EventApiUrl}?pageNumber={pageNumber}&pageSize={pageSize}");

            if (result is null || !result.IsSuccess)
            {
                TempData["ErrorMessage"] = result?.Message ?? "Unable to load events.";
                return View("EventIndex", new PageResult<EventResponseModel>());
            }

            return View("EventIndex", result.Data ?? new PageResult<EventResponseModel>());
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            TempData["ErrorMessage"] = "Unable to connect to the Event API. Please try again.";
            return View("EventIndex", new PageResult<EventResponseModel>());
        }
    }

    public async Task<IActionResult> Details(long id)
    {
        ApiResult<EventDashboardViewModel>? result;
        try
        {
            result = await _httpClient.GetFromJsonAsync<ApiResult<EventDashboardViewModel>>($"{EventApiUrl}/{id}/dashboard");
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            return RedirectToIndexWithError("Unable to load the event dashboard.");
        }

        if (result?.IsSuccess != true || result.Data is null)
            return RedirectToIndexWithError(result?.Message ?? "Event not found.");

        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin")]
    public Task<IActionResult> AssignEventUser(AssignUserToEventRequestModel model) =>
        RunDashboardOperation(HttpMethod.Post, "api/User/assign-user-to-event", model, model.EventId, "people");

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin")]
    public Task<IActionResult> RemoveEventUser(RemoveUserFromEventRequestModel model) =>
        RunDashboardOperation(HttpMethod.Post, "api/User/remove-user-from-event", model, model.EventId, "people");

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin")]
    public Task<IActionResult> AssignEventAdmin(AssignAdminToEventRequestModel model) =>
        RunDashboardOperation(HttpMethod.Post, "api/User/assign-admin-to-event", model, model.EventId, "people");

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin")]
    public Task<IActionResult> RemoveEventAdmin(RemoveAdminFromEventRequestModel model) =>
        RunDashboardOperation(HttpMethod.Post, "api/User/remove-admin-from-event", model, model.EventId, "people");

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> UpdateVotingAllocation(long eventId, VotingLimitUpdateRequestModel model) =>
        RunDashboardOperation(HttpMethod.Patch, "api/Voting/limit", model, eventId, "allocations");

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> CreateEventProject(long eventId, int projectTypeId, string name, string? description) =>
        RunDashboardOperation(HttpMethod.Post, "api/Project", new ProjectCreateRequestModel
        {
            EventId = eventId,
            ProjectTypeId = projectTypeId,
            Name = name,
            Description = description
        }, eventId, "projects");

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> EditEventProject(long eventId, long id, int projectTypeId, string name, string? description) =>
        RunDashboardOperation(HttpMethod.Patch, "api/Project", new ProjectUpdateRequestModel
        {
            Id = id,
            ExpectedEventId = eventId,
            EventId = eventId,
            ProjectTypeId = projectTypeId,
            Name = name,
            Description = description
        }, eventId, "projects");

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> DeleteEventProject(long eventId, long id) =>
        RunDashboardOperation(HttpMethod.Delete, $"api/Project/{id}?eventId={eventId}", null, eventId, "projects");

    public IActionResult Create()
    {
        var now = DateTime.Now;
        var model = new EventCreateRequestModel
        {
            StartDate = now,
            EndDate = now.AddDays(1),
            VotingStartAt = now,
            VotingEndAt = now.AddDays(1)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EventCreateRequestModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var response = await _httpClient.PostAsJsonAsync(EventApiUrl, model);
            var result = await response.Content.ReadFromJsonAsync<ApiResult<EventResponseModel>>();

            if (result?.IsSuccess != true)
            {
                ModelState.AddModelError(string.Empty, result?.Message ?? "Unable to create the event.");
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message ?? "Event created successfully.";
            return RedirectToAction("Index");
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            ModelState.AddModelError(string.Empty, "Unable to connect to the Event API. Please try again.");
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(long id)
    {
        var result = await GetEventById(id);
        if (result?.IsSuccess != true || result.Data is null)
            return RedirectToIndexWithError(result?.Message ?? "Event not found.");

        var model = new EventUpdateRequestModel
        {
            Id = result.Data.Id,
            Name = result.Data.Name,
            Code = result.Data.Code,
            Description = result.Data.Description,
            StartDate = result.Data.EventStartAt,
            EndDate = result.Data.EventEndAt,
            VotingStartAt = result.Data.VotingStartAt,
            VotingEndAt = result.Data.VotingEndAt,
            ResultShowStartAt = result.Data.ResultShowStartAt,
            ResultShowEndAt = result.Data.ResultShowEndAt,
            Status = result.Data.Status
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, EventUpdateRequestModel model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var response = await _httpClient.PatchAsJsonAsync(EventApiUrl, model);
            var result = await response.Content.ReadFromJsonAsync<ApiResult<EventResponseModel>>();

            if (result?.IsSuccess != true)
            {
                ModelState.AddModelError(string.Empty, result?.Message ?? "Unable to update the event.");
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message ?? "Event updated successfully.";
            return RedirectToAction("Index");
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            ModelState.AddModelError(string.Empty, "Unable to connect to the Event API. Please try again.");
            return View(model);
        }
    }

    public async Task<IActionResult> Delete(long id)
    {
        var result = await GetEventById(id);
        if (result?.IsSuccess != true || result.Data is null)
            return RedirectToIndexWithError(result?.Message ?? "Event not found.");

        return View(result.Data);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{EventApiUrl}/{id}");
            var result = await response.Content.ReadFromJsonAsync<ApiResult<EventResponseModel>>();

            TempData[result?.IsSuccess == true ? "SuccessMessage" : "ErrorMessage"] =
                result?.Message ?? "Unable to delete the event.";
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            TempData["ErrorMessage"] = "Unable to connect to the Event API. Please try again.";
        }

        return RedirectToAction("Index");
    }

    private async Task<ApiResult<EventResponseModel>?> GetEventById(long id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ApiResult<EventResponseModel>>($"{EventApiUrl}/{id}");
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            return null;
        }
    }

    private async Task<IActionResult> RunDashboardOperation(
        HttpMethod method,
        string url,
        object? model,
        long eventId,
        string tab)
    {
        try
        {
            using var request = new HttpRequestMessage(method, url);
            if (model != null)
                request.Content = JsonContent.Create(model);

            using var response = await _httpClient.SendAsync(request);
            var result = await response.Content.ReadFromJsonAsync<ApiResult<System.Text.Json.JsonElement>>();
            TempData[result?.IsSuccess == true ? "SuccessMessage" : "ErrorMessage"] =
                result?.Message ?? "The event operation failed.";
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            TempData["ErrorMessage"] = "Unable to connect to the API. Please try again.";
        }

        return Redirect($"{Url.Action(nameof(Details), new { id = eventId })}#{tab}");
    }

    private IActionResult RedirectToIndexWithError(string message)
    {
        TempData["ErrorMessage"] = message;
        return RedirectToAction("Index");
    }

    private static bool IsApiException(Exception ex) =>
        ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException;

    private void LogApiError(Exception ex)
    {
        _logger.LogError(ex, "An Event API request failed.");
    }
}
