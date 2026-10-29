namespace VotingSystem.Domain.Features.User;

public class UserDtos
{
    public long Id { get; set; }
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? PasswordHash { get; set; }
    public int RoleId { get; set; }
    public bool IsActive { get; set; }
    public bool DeleteFlag { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public long? ModifiedBy { get; set; }
}

public class UserCreateRequestModel
{
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? PasswordHash { get; set; }
    public int RoleId { get; set; }
}

public class UserUpdateRequestModel
{
    public long Id { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? PasswordHash { get; set; }
    public int? RoleId { get; set; }
}

public class UserResponseModel
{
    public long Id { get; set; }
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int RoleId { get; set; }
}

public class AssignUserToEventRequestModel
{
    public long UserId { get; set; }
    public long EventId { get; set; }
    public string EventPasswordHash { get; set; } = null!;
    public DateTime PasswordExpiresAt { get; set; }
    public long? AssignedBy { get; set; }
}

public class RemoveUserFromEventRequestModel
{
    public long UserId { get; set; }
    public long EventId { get; set; }
    public long? RemovedBy { get; set; }
}

public class AssignAdminToEventRequestModel
{
    public long AdminUserId { get; set; }
    public long EventId { get; set; }
    public long? AssignedBy { get; set; }
}

public class RemoveAdminFromEventRequestModel
{
    public long AdminUserId { get; set; }
    public long EventId { get; set; }
}

public class UserEventResponseModel
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long EventId { get; set; }
    public DateTime AssignedAt { get; set; }
    public long? AssignedBy { get; set; }
    public DateTime PasswordGeneratedAt { get; set; }
    public DateTime PasswordExpiresAt { get; set; }
    public string AccessStatus { get; set; } = null!;
}

public class AdminEventResponseModel
{
    public long Id { get; set; }
    public long AdminUserId { get; set; }
    public long EventId { get; set; }
    public DateTime AssignedAt { get; set; }
    public long? AssignedBy { get; set; }
}
