using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VotingSystem.Domain.Features.Project;
using VotingSystem.Shared;

namespace VotingSystem.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "SuperAdmin,Admin")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<ActionResult<Result<PageResult<ProjectResponseModel>>>> GetProjects(
        int pageNumber = 1,
        int pageSize = 10,
        string? status = null)
    {
        var response = await _projectService.GetProjectlist(pageNumber, pageSize, status);
        return response.ToActionResult();
    }

    [HttpGet("statuses")]
    public ActionResult<IReadOnlyList<string>> GetProjectStatuses()
    {
        return Ok(Enum.GetNames<Enums.EventStatus>());
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Result<ProjectResponseModel>>> GetProjectById(long id)
    {
        var response = await _projectService.GetProjectByID(id);
        return response.ToActionResult();
    }

    [HttpPost]
    public async Task<ActionResult<Result<ProjectResponseModel>>> CreateProject(
        [FromBody] ProjectCreateRequestModel request)
    {
        var response = await _projectService.CreateProjectAsync(request);
        return response.ToActionResult();
    }

    [HttpPatch]
    public async Task<ActionResult<Result<ProjectResponseModel>>> UpdateProject(
        [FromBody] ProjectUpdateRequestModel request)
    {
        var response = await _projectService.UpdateProject(request);
        return response.ToActionResult();
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<Result<ProjectResponseModel>>> DeleteProject(long id)
    {
        var response = await _projectService.DeleteProject(id);
        return response.ToActionResult();
    }
}
