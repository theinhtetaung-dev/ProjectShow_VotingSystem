namespace VotingSystem.MVC.Models;

public class AssignPermissionToRoleRequestModel { public int RoleId { get; set; } public int PermissionId { get; set; } public long? AssignedBy { get; set; } }
public class RemovePermissionFromRoleRequestModel { public int RoleId { get; set; } public int PermissionId { get; set; } }
public class RolePermissionResponseModel
{
    public long Id { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public int PermissionId { get; set; }
    public string PermissionName { get; set; } = string.Empty;
    public string PermissionCode { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; }
    public long? AssignedBy { get; set; }
}
