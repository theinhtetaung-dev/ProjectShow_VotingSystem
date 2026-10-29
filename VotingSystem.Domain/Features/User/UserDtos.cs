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
