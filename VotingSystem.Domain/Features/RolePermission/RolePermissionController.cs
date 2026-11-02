using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VotingSystem.Domain.Features.RolePermission;
using VotingSystem.Shared;

namespace VotingSystem.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "SuperAdmin")]
public class RolePermissionController : ControllerBase
{
    private readonly IRolePermissionService _rolePermissionService;

    public RolePermissionController(IRolePermissionService rolePermissionService)
    {
        _rolePermissionService = rolePermissionService;
    }

    [HttpGet]
    public async Task<ActionResult<Result<PageResult<RolePermissionResponseModel>>>> GetRolePermissions(int pageNumber = 1, int pageSize = 10)
    {
        var response = await _rolePermissionService.GetRolePermissionlist();
        return response.ToPageResult(pageNumber, pageSize).ToActionResult();
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Result<RolePermissionResponseModel>>> GetRolePermissionById(long id)
    {
        var response = await _rolePermissionService.GetRolePermissionByID(id);
        return response.ToActionResult();
    }

    [HttpGet("role/{roleId:int}")]
    public async Task<ActionResult<Result<List<RolePermissionResponseModel>>>> GetPermissionsByRoleId(int roleId)
    {
        var response = await _rolePermissionService.GetRolePermissions(roleId);
        return response.ToActionResult();
    }

    [HttpPost("assign")]
    public async Task<ActionResult<Result<RolePermissionResponseModel>>> AssignPermissionToRole(
        [FromBody] AssignPermissionToRoleRequestModel request)
    {
        var response = await _rolePermissionService.AssignPermissionToRoleAsync(request);
        return response.ToActionResult();
    }

    [HttpPost("remove")]
    public async Task<ActionResult<Result<RolePermissionResponseModel>>> RemovePermissionFromRole(
        [FromBody] RemovePermissionFromRoleRequestModel request)
    {
        var response = await _rolePermissionService.RemovePermissionFromRoleAsync(request);
        return response.ToActionResult();
    }
}
