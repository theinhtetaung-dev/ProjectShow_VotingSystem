using Microsoft.AspNetCore.Mvc;
using VotingSystem.Domain.Features.Project;
using VotingSystem.Shared;

namespace VotingSystem.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<ActionResult<Result<List<ProjectResponseModel>>>> GetProjects()
    {
        var response = await _projectService.GetProjectlist();
        return response.ToActionResult();
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
