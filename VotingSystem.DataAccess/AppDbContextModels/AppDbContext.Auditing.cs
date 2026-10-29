using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using VotingSystem.DataAccess.Auditing;

namespace VotingSystem.DataAccess.AppDbContextModels;

public partial class AppDbContext
{
    private IAuditActorProvider? AuditActorProvider { get; set; }

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        IAuditActorProvider? auditActorProvider = null)
        : this(options)
    {
        AuditActorProvider = auditActorProvider;
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        var auditEntries = CaptureAuditEntries();
        IDbContextTransaction? transaction = null;

        try
        {
            if (Database.CurrentTransaction == null)
            {
                transaction = Database.BeginTransaction();
            }

            var result = base.SaveChanges(true);
            AddAuditLogs(auditEntries);
            if (auditEntries.Count > 0)
            {
                base.SaveChanges(true);
            }

            transaction?.Commit();
            return result;
        }
        catch
        {
            transaction?.Rollback();
            throw;
        }
        finally
        {
            transaction?.Dispose();
        }
    }

    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        var auditEntries = CaptureAuditEntries();
        IDbContextTransaction? transaction = null;

        try
        {
            if (Database.CurrentTransaction == null)
            {
                transaction = await Database.BeginTransactionAsync(cancellationToken);
            }

            var result = await base.SaveChangesAsync(true, cancellationToken);
            AddAuditLogs(auditEntries);
            if (auditEntries.Count > 0)
            {
                await base.SaveChangesAsync(true, cancellationToken);
            }

            if (transaction != null)
            {
                await transaction.CommitAsync(cancellationToken);
            }

            return result;
        }
        catch
        {
            if (transaction != null)
            {
                await transaction.RollbackAsync(cancellationToken);
            }

            throw;
        }
        finally
        {
            if (transaction != null)
            {
                await transaction.DisposeAsync();
            }
        }
    }

    private List<PendingAuditEntry> CaptureAuditEntries()
    {
        ChangeTracker.DetectChanges();

        return ChangeTracker.Entries()
            .Where(entry => entry.Entity is not TblAuditLog
                && entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Select(entry => new PendingAuditEntry(
                entry,
                entry.State,
                GetAction(entry),
                GetEntityType(entry),
                GetValues(entry, useOriginalValues: true)))
            .ToList();
    }

    private void AddAuditLogs(IEnumerable<PendingAuditEntry> auditEntries)
    {
        var performedAt = DateTime.UtcNow;
        var performedBy = AuditActorProvider?.UserId;

        foreach (var pending in auditEntries)
        {
            var newValues = pending.State == EntityState.Deleted
                ? null
                : GetValues(pending.Entry, useOriginalValues: false);

            TblAuditLogs.Add(new TblAuditLog
            {
                Action = pending.Action,
                EntityType = pending.EntityType,
                EntityId = GetEntityId(pending.Entry),
                PerformedBy = performedBy,
                PerformedAt = performedAt,
                OldValue = SerializeValues(pending.OldValues),
                NewValue = SerializeValues(newValues),
                Description = $"{pending.Action} {pending.EntityType}."
            });
        }
    }

    private static Dictionary<string, object?>? GetValues(
        EntityEntry entry,
        bool useOriginalValues)
    {
        if (entry.State == EntityState.Added && useOriginalValues)
        {
            return null;
        }

        var properties = entry.State == EntityState.Modified
            ? entry.Properties.Where(property => property.IsModified)
            : entry.Properties;

        return properties.ToDictionary(
            property => property.Metadata.Name,
            property => IsSensitive(property.Metadata.Name)
                ? "[REDACTED]"
                : useOriginalValues
                    ? property.OriginalValue
                    : property.CurrentValue);
    }

    private static string GetAction(EntityEntry entry)
    {
        if (entry.State == EntityState.Added)
        {
            if (entry.Entity is TblUserEvent or TblAdminEvent or TblRolePermission)
            {
                return "Assign";
            }

            if (entry.Entity is TblVote)
            {
                return "Vote";
            }

            if (entry.Entity is TblVotingLimit)
            {
                return "UpdateVotingLimit";
            }

            return "Create";
        }

        if (entry.State == EntityState.Deleted)
        {
            return "Delete";
        }

        var deleteFlag = entry.Properties
            .FirstOrDefault(property => property.Metadata.Name == "DeleteFlag");
        if (deleteFlag is { IsModified: true })
        {
            var wasDeleted = deleteFlag.OriginalValue is true;
            var isDeleted = deleteFlag.CurrentValue is true;
            if (!wasDeleted && isDeleted)
            {
                return entry.Entity is TblUserEvent or TblAdminEvent or TblRolePermission
                    ? "Remove"
                    : "Delete";
            }

            if (wasDeleted
                && !isDeleted
                && entry.Entity is TblUserEvent or TblAdminEvent or TblRolePermission)
            {
                return "Assign";
            }
        }

        return entry.Entity is TblVotingLimit ? "UpdateVotingLimit" : "Update";
    }

    private static string GetEntityType(EntityEntry entry)
    {
        var name = entry.Metadata.ClrType.Name;
        return name.StartsWith("Tbl", StringComparison.Ordinal) ? name[3..] : name;
    }

    private static long? GetEntityId(EntityEntry entry)
    {
        var key = entry.Metadata.FindPrimaryKey();
        if (key?.Properties.Count != 1)
        {
            return null;
        }

        var value = entry.Property(key.Properties[0].Name).CurrentValue;
        return value == null ? null : Convert.ToInt64(value);
    }

    private static bool IsSensitive(string propertyName)
    {
        return propertyName.Contains("Password", StringComparison.OrdinalIgnoreCase);
    }

    private static string? SerializeValues(Dictionary<string, object?>? values)
    {
        return values == null ? null : JsonSerializer.Serialize(values);
    }

    private sealed record PendingAuditEntry(
        EntityEntry Entry,
        EntityState State,
        string Action,
        string EntityType,
        Dictionary<string, object?>? OldValues);
}
