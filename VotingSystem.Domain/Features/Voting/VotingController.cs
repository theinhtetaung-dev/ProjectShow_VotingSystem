using Microsoft.AspNetCore.Mvc;
using VotingSystem.Domain.Features.Voting;
using VotingSystem.Shared;

namespace VotingSystem.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VotingController : ControllerBase
{
    private readonly IVotingService _votingService;

    public VotingController(IVotingService votingService)
    {
        _votingService = votingService;
    }

    [HttpGet]
    public async Task<ActionResult<Result<List<VotingResponseModel>>>> GetVotings()
    {
        var response = await _votingService.GetVotinglist();
        return response.ToActionResult();
    }

    [HttpPost("vote")]
    public async Task<ActionResult<Result<VoteResponseModel>>> Vote(
        [FromBody] VoteRequestModel request)
    {
        var response = await _votingService.VoteAsync(request);
        return response.ToActionResult();
    }

    [HttpPatch("limit")]
    public async Task<ActionResult<Result<VotingLimitResponseModel>>> UpdateVotingLimit(
        [FromBody] VotingLimitUpdateRequestModel request)
    {
        var response = await _votingService.UpdateVotingLimitAsync(request);
        return response.ToActionResult();
    }
}
