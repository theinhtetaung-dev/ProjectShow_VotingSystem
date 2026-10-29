using Microsoft.AspNetCore.Mvc;
using VotingSystem.Domain.Features.Permission;
using VotingSystem.Shared;

namespace VotingSystem.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PermissionController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public PermissionController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    [HttpGet]
    public async Task<ActionResult<Result<List<PermissionResponseModel>>>> GetPermissions()
    {
        var response = await _permissionService.GetPermissionlist();
        return response.ToActionResult();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Result<PermissionResponseModel>>> GetPermissionById(int id)
    {
        var response = await _permissionService.GetPermissionByID(id);
        return response.ToActionResult();
    }

    [HttpPost]
    public async Task<ActionResult<Result<PermissionResponseModel>>> CreatePermission(
        [FromBody] PermissionCreateRequestModel request)
    {
        var response = await _permissionService.CreatePermissionAsync(request);
        return response.ToActionResult();
    }

    [HttpPatch]
    public async Task<ActionResult<Result<PermissionResponseModel>>> UpdatePermission(
        [FromBody] PermissionUpdateRequestModel request)
    {
        var response = await _permissionService.UpdatePermission(request);
        return response.ToActionResult();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<Result<PermissionResponseModel>>> DeletePermission(int id)
    {
        var response = await _permissionService.DeletePermission(id);
        return response.ToActionResult();
    }
}
