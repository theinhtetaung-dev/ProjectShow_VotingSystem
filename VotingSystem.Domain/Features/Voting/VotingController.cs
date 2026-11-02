using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using VotingSystem.Domain.Features.Voting;
using VotingSystem.Shared;

namespace VotingSystem.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "SuperAdmin,Admin,User")]
public class VotingController : ControllerBase
{
    private readonly IVotingService _votingService;

    public VotingController(IVotingService votingService)
    {
        _votingService = votingService;
    }

    [HttpGet]
    public async Task<ActionResult<Result<PageResult<VotingResponseModel>>>> GetVotings(int pageNumber = 1, int pageSize = 10)
    {
        var response = await _votingService.GetVotinglist();
        return response.ToPageResult(pageNumber, pageSize).ToActionResult();
    }

    [HttpPost("vote")]
    public async Task<ActionResult<Result<VoteResponseModel>>> Vote(
        [FromBody] VoteRequestModel request)
    {
        var subject = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!long.TryParse(subject, out var userId)) return Unauthorized();

        var response = await _votingService.VoteAsync(request, userId);
        return response.ToActionResult();
    }

    [HttpPatch("limit")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<ActionResult<Result<VotingLimitResponseModel>>> UpdateVotingLimit(
        [FromBody] VotingLimitUpdateRequestModel request)
    {
        var response = await _votingService.UpdateVotingLimitAsync(request);
        return response.ToActionResult();
    }
}
