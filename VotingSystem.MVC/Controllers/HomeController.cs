using System.Diagnostics;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotingSystem.MVC.Models;

namespace VotingSystem.MVC.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public async Task<IActionResult> Index([FromServices] IHttpClientFactory clients, CancellationToken cancellationToken)
    {
        var api = clients.CreateClient("VotingSystemApi");
        var canManage = User.IsInRole("SuperAdmin") || User.IsInRole("Admin");
        var metrics = new List<Task<(string Label, int? Count)>>();
        if (canManage)
        {
            metrics.Add(ReadTotal<EventResponseModel>("Events", "Event"));
            metrics.Add(ReadTotal<ProjectResponseModel>("Projects", "Project"));
        }
        metrics.Add(ReadTotal<VotingResponseModel>("Voting records", "Voting"));
        metrics.Add(ReadTotal<ResultResponseModel>("Results", "Result"));
        ViewData["Metrics"] = await Task.WhenAll(metrics);

        try
        {
            var projectResult = await api.GetFromJsonAsync<ApiResult<PageResult<ProjectResponseModel>>>(
                "api/Project?pageNumber=1&pageSize=6", cancellationToken);
            if (projectResult?.IsSuccess == true && projectResult.Data?.Items?.Count > 0)
            {
                ViewData["TopProjects"] = projectResult.Data.Items;
            }
        }
        catch (Exception ex) when (ex is HttpRequestException or System.Text.Json.JsonException or TaskCanceledException)
        {
            _logger.LogWarning(ex, "Could not load top projects for dashboard");
        }

        return View();

        async Task<(string Label, int? Count)> ReadTotal<T>(string label, string endpoint)
        {
            try
            {
                var result = await api.GetFromJsonAsync<ApiResult<PageResult<T>>>(
                    $"api/{endpoint}?pageNumber=1&pageSize=10", cancellationToken);
                return (label, result?.IsSuccess == true ? result.Data?.TotalCount : null);
            }
            catch (Exception ex) when (ex is HttpRequestException or System.Text.Json.JsonException or TaskCanceledException)
            {
                _logger.LogWarning(ex, "Could not load dashboard metric {Metric}", label);
                return (label, null);
            }
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [AllowAnonymous]
    public IActionResult Error()
    {
        var model = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };

        return View(model);
    }
}
