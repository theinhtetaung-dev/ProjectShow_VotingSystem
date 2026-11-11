using System.Net;
using System.Net.Mail;
using FluentEmail.Smtp;
using Microsoft.Extensions.Options;

namespace VotingSystem.Domain.Features.Email;

public sealed class FluentEmailDispatcher : IFluentEmailDispatcher
{
    private readonly SmtpOptions _smtpOptions;

    public FluentEmailDispatcher(IOptions<SmtpOptions> smtpOptions)
    {
        _smtpOptions = smtpOptions.Value;
    }

    public async Task<(bool IsSuccess, string? ErrorMessage)> SendAsync(
        string fromEmail,
        string? fromName,
        string appPassword,
        EmailSendRequestModel request,
        CancellationToken cancellationToken = default)
    {
        _smtpOptions.Validate();

        var resolvedFromName = string.IsNullOrWhiteSpace(fromName)
            ? _smtpOptions.DefaultFromName
            : fromName;

        var message = FluentEmail.Core.Email
            .From(fromEmail, resolvedFromName)
            .To(request.ToEmail.Trim(), request.ToName?.Trim())
            .Subject(request.Subject.Trim())
            .Body(request.Body, request.IsHtml);

        using var smtpClient = new SmtpClient(_smtpOptions.Host, _smtpOptions.Port)
        {
            EnableSsl = _smtpOptions.EnableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(fromEmail, appPassword),
            Timeout = _smtpOptions.TimeoutSeconds * 1000
        };

        var sender = new SmtpSender(smtpClient);
        var response = await sender.SendAsync(message, cancellationToken);

        if (response.Successful)
        {
            return (true, null);
        }

        var error = response.ErrorMessages is { Count: > 0 }
            ? string.Join("; ", response.ErrorMessages)
            : "SMTP delivery failed without a specific error message.";
        return (false, error);
    }
}
