namespace VotingSystem.Domain.Features.Email;

public interface IFluentEmailDispatcher
{
    Task<(bool IsSuccess, string? ErrorMessage)> SendAsync(
        string fromEmail,
        string? fromName,
        string appPassword,
        EmailSendRequestModel request,
        CancellationToken cancellationToken = default);
}
