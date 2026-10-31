using System.ComponentModel.DataAnnotations;

namespace VotingSystem.MVC.Models;

public class RoleCreateRequestModel
{
    [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
    [StringLength(500)] public string? Description { get; set; }
}

public class RoleUpdateRequestModel : RoleCreateRequestModel
{
    public int Id { get; set; }
}

public class RoleResponseModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
