using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotingSystem.Domain.Features.Email;
using VotingSystem.Shared;

namespace VotingSystem.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "SuperAdmin,Admin")]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public EmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("send")]
    public async Task<ActionResult<Result<EmailSendResponseModel>>> SendEmail(
        [FromBody] EmailSendRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var response = await _emailService.SendEmailAsync(request, cancellationToken);
        return response.ToActionResult();
    }
}
