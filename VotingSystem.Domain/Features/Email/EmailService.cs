using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VotingSystem.DataAccess.AppDbContextModels;
using VotingSystem.Shared;

namespace VotingSystem.Domain.Features.Email;

public sealed class EmailService : IEmailService
{
    private readonly AppDbContext _dbContext;
    private readonly IEmailAccountSelector _accountSelector;
    private readonly IEmailCredentialProtector _credentialProtector;
    private readonly IFluentEmailDispatcher _emailDispatcher;
    private readonly SmtpOptions _smtpOptions;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        AppDbContext dbContext,
        IEmailAccountSelector accountSelector,
        IEmailCredentialProtector credentialProtector,
        IFluentEmailDispatcher emailDispatcher,
        IOptions<SmtpOptions> smtpOptions,
        ILogger<EmailService> logger)
    {
        _dbContext = dbContext;
        _accountSelector = accountSelector;
        _credentialProtector = credentialProtector;
        _emailDispatcher = emailDispatcher;
        _smtpOptions = smtpOptions.Value;
        _logger = logger;
    }

    public async Task<Result<EmailSendResponseModel>> SendEmailAsync(
        EmailSendRequestModel request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            return Result<EmailSendResponseModel>.ValidationError("Email request is required.");
        }

        var recipientEmail = request.ToEmail?.Trim();
        if (!recipientEmail.IsValidEmail(255))
        {
            return Result<EmailSendResponseModel>.ValidationError(
                "Recipient email address is required, must be valid, and must not exceed 255 characters.");
        }

        var recipientName = string.IsNullOrWhiteSpace(request.ToName) ? null : request.ToName.Trim();
        if (!recipientName.IsValidOptionalString(150))
        {
            return Result<EmailSendResponseModel>.ValidationError(
                "Recipient name must not exceed 150 characters.");
        }

        var subject = request.Subject?.Trim();
        if (!subject.IsValidRequiredString(250))
        {
            return Result<EmailSendResponseModel>.ValidationError(
                "Subject is required and must not exceed 250 characters.");
        }

        if (string.IsNullOrWhiteSpace(request.Body))
        {
            return Result<EmailSendResponseModel>.ValidationError("Email body is required.");
        }

        var maxAttempts = Math.Max(1, _smtpOptions.MaxRetryAttempts);
        var attemptedAccountIds = new HashSet<long>();
        string? lastError = null;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var selectionResult = await _accountSelector.SelectNextAccountAsync(
                attemptedAccountIds,
                cancellationToken);

            if (!selectionResult.IsSuccess || selectionResult.Data == null)
            {
                if (attemptedAccountIds.Count == 0)
                {
                    return selectionResult.IsValidationError()
                        ? Result<EmailSendResponseModel>.ValidationError(selectionResult.Message)
                        : Result<EmailSendResponseModel>.SystemError(selectionResult.Message);
                }

                break;
            }

            var selectedAccount = selectionResult.Data;
            attemptedAccountIds.Add(selectedAccount.Id);

            string plainAppPassword;
            try
            {
                plainAppPassword = _credentialProtector.Unprotect(selectedAccount.EncryptedAppPassword);
            }
            catch (CryptographicException ex)
            {
                lastError = "Unable to decrypt account credentials.";
                _logger.LogError(
                    ex,
                    "Attempt {Attempt}: Credential decryption failed for email account {AccountId} ({SenderEmail}).",
                    attempt,
                    selectedAccount.Id,
                    selectedAccount.EmailAddress);
                continue;
            }

            try
            {
                var (isSuccess, errorMessage) = await _emailDispatcher.SendAsync(
                    selectedAccount.EmailAddress,
                    selectedAccount.DisplayName,
                    plainAppPassword,
                    request,
                    cancellationToken);

                if (isSuccess)
                {
                    var sentAtUtc = DateTime.UtcNow;
                    await _dbContext.TblEmailAccounts
                        .Where(x => x.Id == selectedAccount.Id && !x.DeleteFlag)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(x => x.TotalEmailsSent, x => x.TotalEmailsSent + 1)
                            .SetProperty(x => x.LastUsedAt, sentAtUtc),
                            cancellationToken);

                    _logger.LogInformation(
                        "Email sent to {RecipientEmail} via account {AccountId} ({SenderEmail}) on attempt {Attempt}.",
                        recipientEmail,
                        selectedAccount.Id,
                        selectedAccount.EmailAddress,
                        attempt);

                    return Result<EmailSendResponseModel>.Success(new EmailSendResponseModel
                    {
                        EmailAccountId = selectedAccount.Id,
                        SenderEmail = selectedAccount.EmailAddress,
                        RecipientEmail = recipientEmail!,
                        Subject = subject!,
                        SentAtUtc = sentAtUtc,
                        AttemptsMade = attempt
                    }, "Email sent successfully.");
                }

                lastError = errorMessage ?? "SMTP delivery failed.";
                _logger.LogWarning(
                    "Attempt {Attempt}: Failed to send email to {RecipientEmail} via account {AccountId} ({SenderEmail}). Reason: {Reason}",
                    attempt,
                    recipientEmail,
                    selectedAccount.Id,
                    selectedAccount.EmailAddress,
                    lastError);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
                _logger.LogError(
                    ex,
                    "Attempt {Attempt}: Exception while sending email to {RecipientEmail} via account {AccountId} ({SenderEmail}).",
                    attempt,
                    recipientEmail,
                    selectedAccount.Id,
                    selectedAccount.EmailAddress);
            }
        }

        var failureMessage = $"Failed to send email after {attemptedAccountIds.Count} attempt(s). {lastError}".Trim();
        _logger.LogError(
            "Email delivery to {RecipientEmail} exhausted all {Attempts} attempt(s). Last error: {LastError}",
            recipientEmail,
            attemptedAccountIds.Count,
            lastError);

        return Result<EmailSendResponseModel>.SystemError(failureMessage);
    }
}
