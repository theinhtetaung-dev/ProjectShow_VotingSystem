using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using VotingSystem.MVC.Models;

namespace VotingSystem.MVC.Controllers;

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
    public async Task<IActionResult> EventIndex()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<ApiResult<List<EventResponseModel>>>(EventApiUrl);

            if (result is null || !result.IsSuccess)
            {
                TempData["ErrorMessage"] = result?.Message ?? "Unable to load events.";
                return View("EventIndex", new List<EventResponseModel>());
            }

            return View("EventIndex", result.Data ?? new List<EventResponseModel>());
        }
        catch (Exception ex) when (IsApiException(ex))
        {
            LogApiError(ex);
            TempData["ErrorMessage"] = "Unable to connect to the Event API. Please try again.";
            return View("EventIndex", new List<EventResponseModel>());
        }
    }

    public async Task<IActionResult> Details(long id)
    {
        var result = await GetEventById(id);
        if (result?.IsSuccess != true || result.Data is null)
            return RedirectToIndexWithError(result?.Message ?? "Event not found.");

        return View(result.Data);
    }

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
