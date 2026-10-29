using Microsoft.AspNetCore.Mvc;
using VotingSystem.Domain.Features.ProjectType;
using VotingSystem.Shared;

namespace VotingSystem.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectTypeController : ControllerBase
{
    private readonly IProjectTypeService _projectTypeService;

    public ProjectTypeController(IProjectTypeService projectTypeService)
    {
        _projectTypeService = projectTypeService;
    }

    [HttpGet]
    public async Task<ActionResult<Result<List<ProjectTypeResponseModel>>>> GetProjectTypes()
    {
        var response = await _projectTypeService.GetProjectTypelist();
        return response.ToActionResult();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Result<ProjectTypeResponseModel>>> GetProjectTypeById(int id)
    {
        var response = await _projectTypeService.GetProjectTypeByID(id);
        return response.ToActionResult();
    }

    [HttpPost]
    public async Task<ActionResult<Result<ProjectTypeResponseModel>>> CreateProjectType(
        [FromBody] ProjectTypeCreateRequestModel request)
    {
        var response = await _projectTypeService.CreateProjectTypeAsync(request);
        return response.ToActionResult();
    }

    [HttpPatch]
    public async Task<ActionResult<Result<ProjectTypeResponseModel>>> UpdateProjectType(
        [FromBody] ProjectTypeUpdateRequestModel request)
    {
        var response = await _projectTypeService.UpdateProjectType(request);
        return response.ToActionResult();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<Result<ProjectTypeResponseModel>>> DeleteProjectType(int id)
    {
        var response = await _projectTypeService.DeleteProjectType(id);
        return response.ToActionResult();
    }
}
