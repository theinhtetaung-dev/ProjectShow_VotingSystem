using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VotingSystem.Domain.Features.User;
using VotingSystem.Shared;

namespace VotingSystem.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "SuperAdmin,Admin")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<Result<List<UserResponseModel>>>> GetUsers()
    {
        var response = await _userService.GetUserlist();
        return response.ToActionResult();
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Result<UserResponseModel>>> GetUserById(long id)
    {
        var response = await _userService.GetUserByID(id);
        return response.ToActionResult();
    }

    [HttpPost]
    public async Task<ActionResult<Result<UserResponseModel>>> CreateUser(
        [FromBody] UserCreateRequestModel request)
    {
        var response = await _userService.CreateUserAsync(request);
        return response.ToActionResult();
    }

    [HttpPatch]
    public async Task<ActionResult<Result<UserResponseModel>>> UpdateUser(
        [FromBody] UserUpdateRequestModel request)
    {
        var response = await _userService.UpdateUser(request);
        return response.ToActionResult();
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<Result<UserResponseModel>>> DeleteUser(long id)
    {
        var response = await _userService.DeleteUser(id);
        return response.ToActionResult();
    }

    [HttpPost("assign-user-to-event")]
    public async Task<ActionResult<Result<UserEventResponseModel>>> AssignUserToEvent(
        [FromBody] AssignUserToEventRequestModel request)
    {
        var response = await _userService.AssignUserToEventAsync(request);
        return response.ToActionResult();
    }

    [HttpPost("remove-user-from-event")]
    public async Task<ActionResult<Result<UserEventResponseModel>>> RemoveUserFromEvent(
        [FromBody] RemoveUserFromEventRequestModel request)
    {
        var response = await _userService.RemoveUserFromEventAsync(request);
        return response.ToActionResult();
    }

    [HttpPost("assign-admin-to-event")]
    public async Task<ActionResult<Result<AdminEventResponseModel>>> AssignAdminToEvent(
        [FromBody] AssignAdminToEventRequestModel request)
    {
        var response = await _userService.AssignAdminToEventAsync(request);
        return response.ToActionResult();
    }

    [HttpPost("remove-admin-from-event")]
    public async Task<ActionResult<Result<AdminEventResponseModel>>> RemoveAdminFromEvent(
        [FromBody] RemoveAdminFromEventRequestModel request)
    {
        var response = await _userService.RemoveAdminFromEventAsync(request);
        return response.ToActionResult();
    }
}
