namespace VotingSystem.DataAccess.Auditing;

public interface IAuditActorProvider
{
    long? UserId { get; }
}
