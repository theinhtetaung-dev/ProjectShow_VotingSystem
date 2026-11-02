using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VotingSystem.Domain.Features.Role;
using VotingSystem.Shared;

namespace VotingSystem.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "SuperAdmin")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<ActionResult<Result<PageResult<RoleResponseModel>>>> GetRoles(int pageNumber = 1, int pageSize = 10)
    {
        var response = await _roleService.GetRolelist();
        return response.ToPageResult(pageNumber, pageSize).ToActionResult();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Result<RoleResponseModel>>> GetRoleById(int id)
    {
        var response = await _roleService.GetRoleByID(id);
        return response.ToActionResult();
    }

    [HttpPost]
    public async Task<ActionResult<Result<RoleResponseModel>>> CreateRole(
        [FromBody] RoleCreateRequestModel request)
    {
        var response = await _roleService.CreateRoleAsync(request);
        return response.ToActionResult();
    }

    [HttpPatch]
    public async Task<ActionResult<Result<RoleResponseModel>>> UpdateRole(
        [FromBody] RoleUpdateRequestModel request)
    {
        var response = await _roleService.UpdateRole(request);
        return response.ToActionResult();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<Result<RoleResponseModel>>> DeleteRole(int id)
    {
        var response = await _roleService.DeleteRole(id);
        return response.ToActionResult();
    }
}
