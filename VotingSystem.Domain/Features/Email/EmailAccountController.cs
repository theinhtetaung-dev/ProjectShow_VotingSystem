using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotingSystem.Domain.Features.Email;
using VotingSystem.Shared;

namespace VotingSystem.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "SuperAdmin,Admin")]
public class EmailAccountController : ControllerBase
{
    private readonly IEmailAccountService _emailAccountService;

    public EmailAccountController(IEmailAccountService emailAccountService)
    {
        _emailAccountService = emailAccountService;
    }

    [HttpGet]
    public async Task<ActionResult<Result<PageResult<EmailAccountResponseModel>>>> GetEmailAccounts(
        int pageNumber = 1,
        int pageSize = 10,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _emailAccountService.GetEmailAccountsAsync(isActive, cancellationToken);
        return response.ToPageResult(pageNumber, pageSize).ToActionResult();
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Result<EmailAccountResponseModel>>> GetEmailAccountById(
        long id,
        CancellationToken cancellationToken = default)
    {
        var response = await _emailAccountService.GetEmailAccountByIdAsync(id, cancellationToken);
        return response.ToActionResult();
    }

    [HttpPost]
    public async Task<ActionResult<Result<EmailAccountResponseModel>>> CreateEmailAccount(
        [FromBody] EmailAccountCreateRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var response = await _emailAccountService.CreateEmailAccountAsync(request, cancellationToken);
        return response.ToActionResult();
    }

    [HttpPatch]
    public async Task<ActionResult<Result<EmailAccountResponseModel>>> UpdateEmailAccount(
        [FromBody] EmailAccountUpdateRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var response = await _emailAccountService.UpdateEmailAccountAsync(request, cancellationToken);
        return response.ToActionResult();
    }

    [HttpPatch("{id:long}/status")]
    public async Task<ActionResult<Result<EmailAccountResponseModel>>> SetEmailAccountStatus(
        long id,
        [FromBody] EmailAccountStatusUpdateRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var response = await _emailAccountService.SetEmailAccountStatusAsync(id, request.IsActive, cancellationToken);
        return response.ToActionResult();
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<Result<EmailAccountResponseModel>>> DeleteEmailAccount(
        long id,
        CancellationToken cancellationToken = default)
    {
        var response = await _emailAccountService.DeleteEmailAccountAsync(id, cancellationToken);
        return response.ToActionResult();
    }
}
