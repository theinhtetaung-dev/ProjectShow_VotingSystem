using System.ComponentModel.DataAnnotations;

namespace VotingSystem.MVC.Models;

public class PermissionCreateRequestModel
{
    [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
    [StringLength(500)] public string? Description { get; set; }
}

public class PermissionUpdateRequestModel : PermissionCreateRequestModel
{
    public int Id { get; set; }
}

public class PermissionResponseModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
}
