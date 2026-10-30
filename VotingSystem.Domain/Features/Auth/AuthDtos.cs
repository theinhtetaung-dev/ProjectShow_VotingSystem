namespace VotingSystem.Domain.Features.Auth;

public sealed class LoginRequestModel
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? DeviceName { get; set; }
}

public sealed class RefreshTokenRequestModel
{
    public string RefreshToken { get; set; } = null!;
}

public sealed class LogoutRequestModel
{
    public string RefreshToken { get; set; } = null!;
}

public sealed class AuthResponseModel
{
    public string TokenType { get; set; } = "Bearer";
    public string AccessToken { get; set; } = null!;
    public DateTime AccessTokenExpiresAt { get; set; }
    public string RefreshToken { get; set; } = null!;
    public DateTime RefreshTokenExpiresAt { get; set; }
}

public sealed class LogoutResponseModel
{
    public int RevokedTokenCount { get; set; }
}
