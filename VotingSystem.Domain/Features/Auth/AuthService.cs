using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VotingSystem.DataAccess.AppDbContextModels;
using VotingSystem.Shared;

namespace VotingSystem.Domain.Features.Auth;

public sealed class AuthService : IAuthService
{
    public const string PermissionClaimType = "permission";

    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher<TblUser> _passwordHasher;
    private readonly JwtOptions _jwtOptions;

    public AuthService(
        AppDbContext dbContext,
        IPasswordHasher<TblUser> passwordHasher,
        IOptions<JwtOptions> jwtOptions)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<Result<AuthResponseModel>> LoginAsync(
        LoginRequestModel request,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!request.Email.IsValidEmail(255) || !request.Password.IsValidRequiredString(500))
            {
                return InvalidCredentials();
            }

            if (!request.DeviceName.IsValidOptionalString(200))
            {
                return Result<AuthResponseModel>.ValidationError("Device name must not exceed 200 characters.");
            }

            var normalizedEmail = request.Email.Trim();
            var user = await _dbContext.TblUsers
                .Include(x => x.Role)
                .FirstOrDefaultAsync(
                    x => x.Email == normalizedEmail && x.IsActive && !x.DeleteFlag,
                    cancellationToken);

            if (user?.PasswordHash == null)
            {
                return InvalidCredentials();
            }

            PasswordVerificationResult verificationResult;
            try
            {
                verificationResult = _passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    request.Password);
            }
            catch (FormatException)
            {
                return InvalidCredentials();
            }

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return InvalidCredentials();
            }

            if (!user.Role.IsActive || user.Role.DeleteFlag)
            {
                return InvalidCredentials();
            }

            var permissions = await GetPermissionCodesAsync(user.RoleId, cancellationToken);
            var response = CreateTokenPair(
                user,
                permissions,
                Guid.NewGuid(),
                request.DeviceName,
                ipAddress,
                null,
                out var refreshTokenEntity);

            if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
                user.ModifiedAt = DateTime.UtcNow;
            }

            _dbContext.TblRefreshTokens.Add(refreshTokenEntity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<AuthResponseModel>.Success(response, "Login successful.");
        }
        catch (Exception ex)
        {
            return Result<AuthResponseModel>.SystemError(ex.Message);
        }
    }

    public async Task<Result<AuthResponseModel>> RefreshAsync(
        RefreshTokenRequestModel request,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!request.RefreshToken.IsValidRequiredString(1000))
            {
                return InvalidRefreshToken();
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable,
                cancellationToken);

            var tokenHash = HashToken(request.RefreshToken);
            var currentToken = await _dbContext.TblRefreshTokens
                .Include(x => x.User)
                    .ThenInclude(x => x.Role)
                .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

            if (currentToken == null)
            {
                return InvalidRefreshToken();
            }

            var now = DateTime.UtcNow;
            if (currentToken.RevokedAt.HasValue)
            {
                var activeFamilyTokens = await _dbContext.TblRefreshTokens
                    .Where(x => x.TokenFamilyId == currentToken.TokenFamilyId
                        && x.RevokedAt == null
                        && x.ExpiresAt > now)
                    .ToListAsync(cancellationToken);

                foreach (var familyToken in activeFamilyTokens)
                {
                    RevokeToken(familyToken, now, ipAddress, "Refresh token reuse detected.");
                }

                if (activeFamilyTokens.Count > 0)
                {
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);
                return InvalidRefreshToken();
            }

            if (currentToken.ExpiresAt <= now
                || !currentToken.User.IsActive
                || currentToken.User.DeleteFlag
                || !currentToken.User.Role.IsActive
                || currentToken.User.Role.DeleteFlag)
            {
                return InvalidRefreshToken();
            }

            var permissions = await GetPermissionCodesAsync(currentToken.User.RoleId, cancellationToken);
            var response = CreateTokenPair(
                currentToken.User,
                permissions,
                currentToken.TokenFamilyId,
                currentToken.DeviceName,
                ipAddress,
                currentToken.ExpiresAt,
                out var replacementToken);

            currentToken.LastUsedAt = now;
            RevokeToken(currentToken, now, ipAddress, "Rotated.");
            currentToken.ReplacedByToken = replacementToken;
            _dbContext.TblRefreshTokens.Add(replacementToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result<AuthResponseModel>.Success(response, "Token refreshed successfully.");
        }
        catch (Exception ex)
        {
            return Result<AuthResponseModel>.SystemError(ex.Message);
        }
    }

    public async Task<Result<LogoutResponseModel>> LogoutAsync(
        LogoutRequestModel request,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!request.RefreshToken.IsValidRequiredString(1000))
            {
                return Result<LogoutResponseModel>.Success(
                    new LogoutResponseModel(),
                    "Logout successful.");
            }

            var tokenHash = HashToken(request.RefreshToken);
            var refreshToken = await _dbContext.TblRefreshTokens
                .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

            var revokedCount = 0;
            if (refreshToken is { RevokedAt: null })
            {
                RevokeToken(refreshToken, DateTime.UtcNow, ipAddress, "Logged out.");
                revokedCount = await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return Result<LogoutResponseModel>.Success(
                new LogoutResponseModel { RevokedTokenCount = revokedCount > 0 ? 1 : 0 },
                "Logout successful.");
        }
        catch (Exception ex)
        {
            return Result<LogoutResponseModel>.SystemError(ex.Message);
        }
    }

    public async Task<Result<LogoutResponseModel>> LogoutAllAsync(
        long userId,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var now = DateTime.UtcNow;
            var activeTokens = await _dbContext.TblRefreshTokens
                .Where(x => x.UserId == userId && x.RevokedAt == null && x.ExpiresAt > now)
                .ToListAsync(cancellationToken);

            foreach (var token in activeTokens)
            {
                RevokeToken(token, now, ipAddress, "Logged out from all devices.");
            }

            if (activeTokens.Count > 0)
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return Result<LogoutResponseModel>.Success(
                new LogoutResponseModel { RevokedTokenCount = activeTokens.Count },
                "Logged out from all devices successfully.");
        }
        catch (Exception ex)
        {
            return Result<LogoutResponseModel>.SystemError(ex.Message);
        }
    }

    private async Task<List<string>> GetPermissionCodesAsync(
        int roleId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.TblRolePermissions
            .Where(x => x.RoleId == roleId
                && !x.DeleteFlag
                && x.Permission.IsActive
                && !x.Permission.DeleteFlag)
            .Select(x => x.Permission.Code)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(cancellationToken);
    }

    private AuthResponseModel CreateTokenPair(
        TblUser user,
        IEnumerable<string> permissions,
        Guid tokenFamilyId,
        string? deviceName,
        string? ipAddress,
        DateTime? refreshTokenExpiresAt,
        out TblRefreshToken refreshTokenEntity)
    {
        _jwtOptions.Validate();

        var now = DateTime.UtcNow;
        var accessTokenExpiresAt = now.AddMinutes(_jwtOptions.AccessTokenMinutes);
        var refreshExpiresAt = refreshTokenExpiresAt
            ?? now.AddDays(_jwtOptions.RefreshTokenDays);
        var refreshToken = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(64));

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Name, user.Name),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("role", user.Role.Name)
        };
        claims.AddRange(permissions.Select(code => new Claim(PermissionClaimType, code)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            IssuedAt = now,
            NotBefore = now,
            Expires = accessTokenExpiresAt,
            SigningCredentials = new SigningCredentials(
                _jwtOptions.CreateSecurityKey(),
                SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var accessToken = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

        refreshTokenEntity = new TblRefreshToken
        {
            UserId = user.Id,
            TokenHash = HashToken(refreshToken),
            TokenFamilyId = tokenFamilyId,
            ExpiresAt = refreshExpiresAt,
            CreatedAt = now,
            CreatedByIp = ipAddress,
            DeviceName = deviceName
        };

        return new AuthResponseModel
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessTokenExpiresAt,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = refreshExpiresAt
        };
    }

    private static string HashToken(string token)
    {
        return Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token)));
    }

    private static void RevokeToken(
        TblRefreshToken token,
        DateTime revokedAt,
        string? ipAddress,
        string reason)
    {
        token.RevokedAt = revokedAt;
        token.RevokedByIp = ipAddress;
        token.RevocationReason = reason;
    }

    private static Result<AuthResponseModel> InvalidCredentials()
    {
        return Result<AuthResponseModel>.ValidationError("Invalid email or password.");
    }

    private static Result<AuthResponseModel> InvalidRefreshToken()
    {
        return Result<AuthResponseModel>.ValidationError("Invalid or expired refresh token.");
    }
}
