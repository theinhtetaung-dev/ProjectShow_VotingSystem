using System.ComponentModel.DataAnnotations;

namespace VotingSystem.MVC.Models;

public class UserCreateRequestModel
{
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required, StringLength(200)] public string Name { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    [Range(1, int.MaxValue)] public int RoleId { get; set; }
}

public class UserUpdateRequestModel : UserCreateRequestModel
{
    public long Id { get; set; }
}

public class UserResponseModel
{
    public long Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int RoleId { get; set; }
}

public class AssignUserToEventRequestModel
{
    public long UserId { get; set; }
    public long EventId { get; set; }
    [Required] public string EventPasswordHash { get; set; } = string.Empty;
    public DateTime PasswordExpiresAt { get; set; }
    public long? AssignedBy { get; set; }
}

public class RemoveUserFromEventRequestModel { public long UserId { get; set; } public long EventId { get; set; } public long? RemovedBy { get; set; } }
public class AssignAdminToEventRequestModel { public long AdminUserId { get; set; } public long EventId { get; set; } public long? AssignedBy { get; set; } }
public class RemoveAdminFromEventRequestModel { public long AdminUserId { get; set; } public long EventId { get; set; } }
