using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VotingSystem.DataAccess.AppDbContextModels;
using VotingSystem.DataAccess.Auditing;
using VotingSystem.Shared;

namespace VotingSystem.Domain.Features.Email;

public sealed class EmailAccountService : IEmailAccountService
{
    private readonly AppDbContext _dbContext;
    private readonly IEmailCredentialProtector _credentialProtector;
    private readonly IAuditActorProvider _auditActorProvider;
    private readonly ILogger<EmailAccountService> _logger;

    public EmailAccountService(
        AppDbContext dbContext,
        IEmailCredentialProtector credentialProtector,
        IAuditActorProvider auditActorProvider,
        ILogger<EmailAccountService> logger)
    {
        _dbContext = dbContext;
        _credentialProtector = credentialProtector;
        _auditActorProvider = auditActorProvider;
        _logger = logger;
    }

    public async Task<Result<List<EmailAccountResponseModel>>> GetEmailAccountsAsync(
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _dbContext.TblEmailAccounts
                .AsNoTracking()
                .Where(x => !x.DeleteFlag);

            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }

            var accounts = await query
                .OrderBy(x => x.PriorityOrder)
                .ThenBy(x => x.Id)
                .Select(ResponseProjection)
                .ToListAsync(cancellationToken);

            var message = accounts.Count == 0 ? "No email accounts found." : "Success";
            return Result<List<EmailAccountResponseModel>>.Success(accounts, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve email accounts.");
            return Result<List<EmailAccountResponseModel>>.SystemError("Unable to load email accounts.");
        }
    }

    public async Task<Result<EmailAccountResponseModel>> GetEmailAccountByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!id.IsPositive())
            {
                return Result<EmailAccountResponseModel>.ValidationError("Email account ID must be greater than zero.");
            }

            var account = await _dbContext.TblEmailAccounts
                .AsNoTracking()
                .Where(x => x.Id == id && !x.DeleteFlag)
                .Select(ResponseProjection)
                .FirstOrDefaultAsync(cancellationToken);

            if (account == null)
            {
                return Result<EmailAccountResponseModel>.NotFound("Email account not found.");
            }

            return Result<EmailAccountResponseModel>.Success(account, "Success");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve email account {AccountId}.", id);
            return Result<EmailAccountResponseModel>.SystemError("Unable to load email account details.");
        }
    }

    public async Task<Result<EmailAccountResponseModel>> CreateEmailAccountAsync(
        EmailAccountCreateRequestModel request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (request == null)
            {
                return Result<EmailAccountResponseModel>.ValidationError("Request payload is required.");
            }

            var normalizedEmail = request.EmailAddress?.Trim().ToLowerInvariant();
            if (!normalizedEmail.IsValidEmail(255))
            {
                return Result<EmailAccountResponseModel>.ValidationError(
                    "Email address is required, must be valid, and must not exceed 255 characters.");
            }

            var displayName = string.IsNullOrWhiteSpace(request.DisplayName)
                ? null
                : request.DisplayName.Trim();
            if (!displayName.IsValidOptionalString(150))
            {
                return Result<EmailAccountResponseModel>.ValidationError(
                    "Display name must not exceed 150 characters.");
            }

            var appPassword = request.AppPassword?.Trim();
            if (!appPassword.IsValidRequiredString(256))
            {
                return Result<EmailAccountResponseModel>.ValidationError(
                    "App password is required and must not exceed 256 characters.");
            }

            if (!request.PriorityOrder.IsPositive())
            {
                return Result<EmailAccountResponseModel>.ValidationError(
                    "Priority order must be greater than zero.");
            }

            var duplicateExists = await _dbContext.TblEmailAccounts
                .AnyAsync(x => !x.DeleteFlag && x.EmailAddress == normalizedEmail, cancellationToken);
            if (duplicateExists)
            {
                return Result<EmailAccountResponseModel>.ValidationError(
                    "An email account with this address already exists.");
            }

            var now = DateTime.UtcNow;
            var newAccount = new TblEmailAccount
            {
                EmailAddress = normalizedEmail!,
                DisplayName = displayName,
                EncryptedAppPassword = _credentialProtector.Protect(appPassword!),
                PriorityOrder = request.PriorityOrder,
                IsActive = request.IsActive,
                TotalEmailsSent = 0,
                LastUsedAt = null,
                DeleteFlag = false,
                CreatedAt = now,
                CreatedBy = _auditActorProvider.UserId
            };

            _dbContext.TblEmailAccounts.Add(newAccount);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Created email account {AccountId} ({EmailAddress}) with priority {PriorityOrder} (Active={IsActive}).",
                newAccount.Id,
                newAccount.EmailAddress,
                newAccount.PriorityOrder,
                newAccount.IsActive);

            return Result<EmailAccountResponseModel>.Success(
                MapToResponse(newAccount),
                "Email account created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create email account.");
            return Result<EmailAccountResponseModel>.SystemError("Unable to create email account.");
        }
    }

    public async Task<Result<EmailAccountResponseModel>> UpdateEmailAccountAsync(
        EmailAccountUpdateRequestModel request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (request == null || !request.Id.IsPositive())
            {
                return Result<EmailAccountResponseModel>.ValidationError("Email account ID must be greater than zero.");
            }

            var account = await _dbContext.TblEmailAccounts
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.DeleteFlag, cancellationToken);
            if (account == null)
            {
                return Result<EmailAccountResponseModel>.NotFound("Email account not found.");
            }

            var hasChanges = false;

            if (request.EmailAddress != null)
            {
                var normalizedEmail = request.EmailAddress.Trim().ToLowerInvariant();
                if (!normalizedEmail.IsValidEmail(255))
                {
                    return Result<EmailAccountResponseModel>.ValidationError(
                        "Email address must be valid and must not exceed 255 characters.");
                }

                var duplicateExists = await _dbContext.TblEmailAccounts
                    .AnyAsync(
                        x => !x.DeleteFlag && x.EmailAddress == normalizedEmail && x.Id != request.Id,
                        cancellationToken);
                if (duplicateExists)
                {
                    return Result<EmailAccountResponseModel>.ValidationError(
                        "An email account with this address already exists.");
                }

                if (!string.Equals(account.EmailAddress, normalizedEmail, StringComparison.Ordinal))
                {
                    account.EmailAddress = normalizedEmail;
                    hasChanges = true;
                }
            }

            if (request.DisplayName != null)
            {
                var displayName = string.IsNullOrWhiteSpace(request.DisplayName)
                    ? null
                    : request.DisplayName.Trim();
                if (!displayName.IsValidOptionalString(150))
                {
                    return Result<EmailAccountResponseModel>.ValidationError(
                        "Display name must not exceed 150 characters.");
                }

                if (!string.Equals(account.DisplayName, displayName, StringComparison.Ordinal))
                {
                    account.DisplayName = displayName;
                    hasChanges = true;
                }
            }

            if (!string.IsNullOrWhiteSpace(request.AppPassword))
            {
                var trimmedPassword = request.AppPassword.Trim();
                if (!trimmedPassword.IsValidRequiredString(256))
                {
                    return Result<EmailAccountResponseModel>.ValidationError(
                        "App password must not exceed 256 characters.");
                }

                account.EncryptedAppPassword = _credentialProtector.Protect(trimmedPassword);
                hasChanges = true;
            }

            if (request.PriorityOrder.HasValue)
            {
                if (!request.PriorityOrder.Value.IsPositive())
                {
                    return Result<EmailAccountResponseModel>.ValidationError(
                        "Priority order must be greater than zero.");
                }

                if (account.PriorityOrder != request.PriorityOrder.Value)
                {
                    account.PriorityOrder = request.PriorityOrder.Value;
                    hasChanges = true;
                }
            }

            if (request.IsActive.HasValue && account.IsActive != request.IsActive.Value)
            {
                account.IsActive = request.IsActive.Value;
                hasChanges = true;
            }

            if (hasChanges)
            {
                account.ModifiedAt = DateTime.UtcNow;
                account.ModifiedBy = _auditActorProvider.UserId;
                await _dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Updated email account {AccountId} ({EmailAddress}).",
                    account.Id,
                    account.EmailAddress);
            }

            return Result<EmailAccountResponseModel>.Success(
                MapToResponse(account),
                "Email account updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update email account {AccountId}.", request?.Id);
            return Result<EmailAccountResponseModel>.SystemError("Unable to update email account.");
        }
    }

    public async Task<Result<EmailAccountResponseModel>> SetEmailAccountStatusAsync(
        long id,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!id.IsPositive())
            {
                return Result<EmailAccountResponseModel>.ValidationError("Email account ID must be greater than zero.");
            }

            var account = await _dbContext.TblEmailAccounts
                .FirstOrDefaultAsync(x => x.Id == id && !x.DeleteFlag, cancellationToken);
            if (account == null)
            {
                return Result<EmailAccountResponseModel>.NotFound("Email account not found.");
            }

            if (account.IsActive != isActive)
            {
                account.IsActive = isActive;
                account.ModifiedAt = DateTime.UtcNow;
                account.ModifiedBy = _auditActorProvider.UserId;
                await _dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Email account {AccountId} ({EmailAddress}) status changed to Active={IsActive}.",
                    account.Id,
                    account.EmailAddress,
                    account.IsActive);
            }

            var message = isActive
                ? "Email account enabled successfully."
                : "Email account disabled successfully.";
            return Result<EmailAccountResponseModel>.Success(MapToResponse(account), message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update status for email account {AccountId}.", id);
            return Result<EmailAccountResponseModel>.SystemError("Unable to update email account status.");
        }
    }

    public async Task<Result<EmailAccountResponseModel>> DeleteEmailAccountAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!id.IsPositive())
            {
                return Result<EmailAccountResponseModel>.ValidationError("Email account ID must be greater than zero.");
            }

            var account = await _dbContext.TblEmailAccounts
                .FirstOrDefaultAsync(x => x.Id == id && !x.DeleteFlag, cancellationToken);
            if (account == null)
            {
                return Result<EmailAccountResponseModel>.NotFound("Email account not found.");
            }

            account.DeleteFlag = true;
            account.IsActive = false;
            account.ModifiedAt = DateTime.UtcNow;
            account.ModifiedBy = _auditActorProvider.UserId;
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Soft-deleted email account {AccountId} ({EmailAddress}).",
                account.Id,
                account.EmailAddress);

            return Result<EmailAccountResponseModel>.DeleteSuccess("Email account deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete email account {AccountId}.", id);
            return Result<EmailAccountResponseModel>.SystemError("Unable to delete email account.");
        }
    }

    private static readonly Expression<Func<TblEmailAccount, EmailAccountResponseModel>> ResponseProjection =
        account => new EmailAccountResponseModel
        {
            Id = account.Id,
            EmailAddress = account.EmailAddress,
            DisplayName = account.DisplayName,
            PriorityOrder = account.PriorityOrder,
            IsActive = account.IsActive,
            TotalEmailsSent = account.TotalEmailsSent,
            LastUsedAt = account.LastUsedAt,
            CreatedAt = account.CreatedAt,
            UpdatedAt = account.ModifiedAt,
            HasCredential = !string.IsNullOrEmpty(account.EncryptedAppPassword)
        };

    private static EmailAccountResponseModel MapToResponse(TblEmailAccount account)
    {
        return new EmailAccountResponseModel
        {
            Id = account.Id,
            EmailAddress = account.EmailAddress,
            DisplayName = account.DisplayName,
            PriorityOrder = account.PriorityOrder,
            IsActive = account.IsActive,
            TotalEmailsSent = account.TotalEmailsSent,
            LastUsedAt = account.LastUsedAt,
            CreatedAt = account.CreatedAt,
            UpdatedAt = account.ModifiedAt,
            HasCredential = !string.IsNullOrEmpty(account.EncryptedAppPassword)
        };
    }
}
