using VotingSystem.Shared;

namespace VotingSystem.Domain.Features.Auth;

public interface IAuthService
{
    Task<Result<AuthResponseModel>> LoginAsync(
        LoginRequestModel request,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    Task<Result<AuthResponseModel>> RefreshAsync(
        RefreshTokenRequestModel request,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    Task<Result<LogoutResponseModel>> LogoutAsync(
        LogoutRequestModel request,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    Task<Result<LogoutResponseModel>> LogoutAllAsync(
        long userId,
        string? ipAddress,
        CancellationToken cancellationToken = default);
}
