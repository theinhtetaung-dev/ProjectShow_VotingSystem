using System.ComponentModel.DataAnnotations;

namespace VotingSystem.MVC.Models;

public class LoginRequestModel { [Required, EmailAddress] public string Email { get; set; } = string.Empty; [Required, DataType(DataType.Password)] public string Password { get; set; } = string.Empty; public string? DeviceName { get; set; } }
public class RefreshTokenRequestModel { [Required] public string RefreshToken { get; set; } = string.Empty; }
public class LogoutRequestModel { [Required] public string RefreshToken { get; set; } = string.Empty; }
public class AuthResponseModel { public string TokenType { get; set; } = "Bearer"; public string AccessToken { get; set; } = string.Empty; public DateTime AccessTokenExpiresAt { get; set; } public string RefreshToken { get; set; } = string.Empty; public DateTime RefreshTokenExpiresAt { get; set; } public long UserId { get; set; } public string Email { get; set; } = string.Empty; public string Name { get; set; } = string.Empty; public string Role { get; set; } = string.Empty; }
