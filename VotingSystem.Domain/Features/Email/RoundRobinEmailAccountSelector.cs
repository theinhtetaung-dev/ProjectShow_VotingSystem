using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VotingSystem.DataAccess.AppDbContextModels;
using VotingSystem.Shared;

namespace VotingSystem.Domain.Features.Email;

public sealed class RoundRobinEmailAccountSelector : IEmailAccountSelector
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<RoundRobinEmailAccountSelector> _logger;

    public RoundRobinEmailAccountSelector(
        AppDbContext dbContext,
        ILogger<RoundRobinEmailAccountSelector> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<SelectedEmailAccount>> SelectNextAccountAsync(
        IReadOnlyCollection<long>? excludedAccountIds = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var transaction = await _dbContext.Database
                .BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

            await EnsureStateRowExistsAsync(cancellationToken);

            var stateRows = await _dbContext.TblEmailRoundRobinStates
                .FromSqlRaw(
                    """
                    SELECT id, last_selected_account_id, last_selected_order, updated_at
                    FROM dbo.Tbl_Email_Round_Robin_State WITH (UPDLOCK, HOLDLOCK, ROWLOCK)
                    WHERE id = 1
                    """)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var state = stateRows.FirstOrDefault();

            var activeQuery = _dbContext.TblEmailAccounts
                .AsNoTracking()
                .Where(x => x.IsActive && !x.DeleteFlag);

            if (excludedAccountIds is { Count: > 0 })
            {
                activeQuery = activeQuery.Where(x => !excludedAccountIds.Contains(x.Id));
            }

            var activeAccounts = await activeQuery
                .OrderBy(x => x.PriorityOrder)
                .ThenBy(x => x.Id)
                .Select(x => new SelectedEmailAccount(
                    x.Id,
                    x.EmailAddress,
                    x.DisplayName,
                    x.EncryptedAppPassword,
                    x.PriorityOrder))
                .ToListAsync(cancellationToken);

            if (activeAccounts.Count == 0)
            {
                await transaction.CommitAsync(cancellationToken);
                return Result<SelectedEmailAccount>.ValidationError(
                    "No active email accounts are available for sending.");
            }

            var selectedAccount = DetermineNextAccount(
                activeAccounts,
                state?.LastSelectedOrder,
                state?.LastSelectedAccountId);

            var now = DateTime.UtcNow;
            await _dbContext.Database.ExecuteSqlInterpolatedAsync(
                $"""
                UPDATE dbo.Tbl_Email_Round_Robin_State
                SET last_selected_account_id = {selectedAccount.Id},
                    last_selected_order = {selectedAccount.PriorityOrder},
                    updated_at = {now}
                WHERE id = 1;
                """,
                cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger.LogDebug(
                "Round Robin selected email account {AccountId} ({EmailAddress}, priority {PriorityOrder}).",
                selectedAccount.Id,
                selectedAccount.EmailAddress,
                selectedAccount.PriorityOrder);

            return Result<SelectedEmailAccount>.Success(selectedAccount);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to select next email account via Round Robin.");
            return Result<SelectedEmailAccount>.SystemError("Unable to select an email account.");
        }
    }

    private Task EnsureStateRowExistsAsync(CancellationToken cancellationToken)
    {
        return _dbContext.Database.ExecuteSqlRawAsync(
            """
            IF NOT EXISTS (SELECT 1 FROM dbo.Tbl_Email_Round_Robin_State WITH (UPDLOCK, HOLDLOCK) WHERE id = 1)
            BEGIN
                INSERT INTO dbo.Tbl_Email_Round_Robin_State (id, last_selected_account_id, last_selected_order, updated_at)
                VALUES (1, NULL, NULL, SYSUTCDATETIME());
            END;
            """,
            cancellationToken);
    }

    private static SelectedEmailAccount DetermineNextAccount(
        IReadOnlyList<SelectedEmailAccount> activeAccounts,
        int? lastSelectedOrder,
        long? lastSelectedAccountId)
    {
        if (!lastSelectedOrder.HasValue || !lastSelectedAccountId.HasValue)
        {
            return activeAccounts[0];
        }

        foreach (var account in activeAccounts)
        {
            if (account.PriorityOrder > lastSelectedOrder.Value
                || (account.PriorityOrder == lastSelectedOrder.Value && account.Id > lastSelectedAccountId.Value))
            {
                return account;
            }
        }

        // Wrap back to the first active account after reaching the end of the priority order.
        return activeAccounts[0];
    }
}
