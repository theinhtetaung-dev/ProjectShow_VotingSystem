using System.ComponentModel.DataAnnotations;

namespace VotingSystem.MVC.Models;

public class ProjectTypeCreateRequestModel
{
    [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
    [Required, StringLength(50)] public string Code { get; set; } = string.Empty;
}

public class ProjectTypeUpdateRequestModel : ProjectTypeCreateRequestModel
{
    public int Id { get; set; }
}

public class ProjectTypeResponseModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}
