using Microsoft.AspNetCore.Mvc;
using VotingSystem.Domain.Features.Result;
using VotingSystem.Shared;

namespace VotingSystem.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ResultController : ControllerBase
{
    private readonly IResultService _resultService;

    public ResultController(IResultService resultService)
    {
        _resultService = resultService;
    }

    [HttpGet]
    public async Task<ActionResult<Result<List<ResultResponseModel>>>> GetResults()
    {
        var response = await _resultService.GetResultlist();
        return response.ToActionResult();
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Result<ResultResponseModel>>> GetResultById(long id)
    {
        var response = await _resultService.GetResultByID(id);
        return response.ToActionResult();
    }

    [HttpPost]
    public async Task<ActionResult<Result<ResultResponseModel>>> CreateResult(
        [FromBody] ResultCreateRequestModel request)
    {
        var response = await _resultService.CreateResultAsync(request);
        return response.ToActionResult();
    }

    [HttpPatch]
    public async Task<ActionResult<Result<ResultResponseModel>>> UpdateResult(
        [FromBody] ResultUpdateRequestModel request)
    {
        var response = await _resultService.UpdateResult(request);
        return response.ToActionResult();
    }
}
