using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotingSystem.Domain.Features.Auth;
using VotingSystem.Shared;

namespace VotingSystem.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<Result<AuthResponseModel>>> Login(
        [FromBody] LoginRequestModel request,
        CancellationToken cancellationToken)
    {
        var response = await _authService.LoginAsync(
            request,
            GetIpAddress(),
            cancellationToken);
        return ToAuthActionResult(response);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<Result<AuthResponseModel>>> Refresh(
        [FromBody] RefreshTokenRequestModel request,
        CancellationToken cancellationToken)
    {
        var response = await _authService.RefreshAsync(
            request,
            GetIpAddress(),
            cancellationToken);
        return ToAuthActionResult(response);
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<ActionResult<Result<LogoutResponseModel>>> Logout(
        [FromBody] LogoutRequestModel request,
        CancellationToken cancellationToken)
    {
        var response = await _authService.LogoutAsync(
            request,
            GetIpAddress(),
            cancellationToken);
        return response.ToActionResult();
    }

    [Authorize]
    [HttpPost("logout-all")]
    public async Task<ActionResult<Result<LogoutResponseModel>>> LogoutAll(
        CancellationToken cancellationToken)
    {
        var subject = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!long.TryParse(subject, out var userId))
        {
            return Unauthorized();
        }

        var response = await _authService.LogoutAllAsync(
            userId,
            GetIpAddress(),
            cancellationToken);
        return response.ToActionResult();
    }

    private string? GetIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }

    private ActionResult<Result<AuthResponseModel>> ToAuthActionResult(
        Result<AuthResponseModel> response)
    {
        if (response.IsValidationError()
            && (response.Message == "Invalid email or password."
                || response.Message == "Invalid or expired refresh token."))
        {
            return Unauthorized(response);
        }

        return response.ToActionResult();
    }
}
