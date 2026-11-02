using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VotingSystem.Domain.Features.Result;
using VotingSystem.Shared;

namespace VotingSystem.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "SuperAdmin,Admin,User")]
public class ResultController : ControllerBase
{
    private readonly IResultService _resultService;

    public ResultController(IResultService resultService)
    {
        _resultService = resultService;
    }

    [HttpGet]
    public async Task<ActionResult<Result<PageResult<ResultResponseModel>>>> GetResults(int pageNumber = 1, int pageSize = 10)
    {
        var response = await _resultService.GetResultlist();
        return response.ToPageResult(pageNumber, pageSize).ToActionResult();
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Result<ResultResponseModel>>> GetResultById(long id)
    {
        var response = await _resultService.GetResultByID(id);
        return response.ToActionResult();
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<ActionResult<Result<ResultResponseModel>>> CreateResult(
        [FromBody] ResultCreateRequestModel request)
    {
        var response = await _resultService.CreateResultAsync(request);
        return response.ToActionResult();
    }

    [HttpPatch]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<ActionResult<Result<ResultResponseModel>>> UpdateResult(
        [FromBody] ResultUpdateRequestModel request)
    {
        var response = await _resultService.UpdateResult(request);
        return response.ToActionResult();
    }
}
