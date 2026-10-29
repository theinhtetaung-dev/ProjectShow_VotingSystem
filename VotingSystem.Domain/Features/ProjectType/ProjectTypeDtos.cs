namespace VotingSystem.Domain.Features.ProjectType;

public class ProjectTypeDtos
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public bool IsActive { get; set; }
    public bool DeleteFlag { get; set; }
}

public class ProjectTypeCreateRequestModel
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
}

public class ProjectTypeUpdateRequestModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
}

public class ProjectTypeResponseModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
}
