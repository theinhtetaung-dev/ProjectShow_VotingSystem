using System.ComponentModel.DataAnnotations;

namespace VotingSystem.MVC.Models;

public class ProjectCreateRequestModel
{
    [Range(1, long.MaxValue)] public long EventId { get; set; }
    [Range(1, int.MaxValue)] public int ProjectTypeId { get; set; }
    [Required, StringLength(200)] public string Name { get; set; } = string.Empty;
    [StringLength(1000)] public string? Description { get; set; }
}

public class ProjectUpdateRequestModel : ProjectCreateRequestModel
{
    public long Id { get; set; }
}

public class ProjectResponseModel
{
    public long Id { get; set; }
    public long EventId { get; set; }
    public int ProjectTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int VoteCounts { get; set; }
}
