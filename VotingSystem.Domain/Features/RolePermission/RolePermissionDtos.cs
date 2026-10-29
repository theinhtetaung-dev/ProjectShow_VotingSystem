namespace VotingSystem.Domain.Features.RolePermission;

public class AssignPermissionToRoleRequestModel
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
    public long? AssignedBy { get; set; }
}

public class RemovePermissionFromRoleRequestModel
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
}

public class RolePermissionResponseModel
{
    public long Id { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; } = null!;
    public int PermissionId { get; set; }
    public string PermissionName { get; set; } = null!;
    public string PermissionCode { get; set; } = null!;
    public DateTime AssignedAt { get; set; }
    public long? AssignedBy { get; set; }
}
