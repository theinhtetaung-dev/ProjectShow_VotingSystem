namespace VotingSystem.Domain.Features.Project;

public class ProjectDtos
{
    public long Id { get; set; }
    public long EventId { get; set; }
    public int ProjectTypeId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int VoteCounts { get; set; }
    public bool IsActive { get; set; }
    public bool DeleteFlag { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public long? ModifiedBy { get; set; }
}

public class ProjectCreateRequestModel
{
    public long EventId { get; set; }
    public int ProjectTypeId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

public class ProjectUpdateRequestModel
{
    public long Id { get; set; }
    public long? EventId { get; set; }
    public int? ProjectTypeId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class ProjectResponseModel
{
    public long Id { get; set; }
    public long EventId { get; set; }
    public int ProjectTypeId { get; set; }
    public string EventCode { get; set; } = null!;
    public string ProjectTypeName { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int VoteCounts { get; set; }
}
