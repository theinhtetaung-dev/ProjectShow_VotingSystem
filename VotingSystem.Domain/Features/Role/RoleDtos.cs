namespace VotingSystem.Domain.Features.Role;

public class RoleDtos
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public bool DeleteFlag { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public long? ModifiedBy { get; set; }
}

public class RoleCreateRequestModel
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

public class RoleUpdateRequestModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class RoleResponseModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
