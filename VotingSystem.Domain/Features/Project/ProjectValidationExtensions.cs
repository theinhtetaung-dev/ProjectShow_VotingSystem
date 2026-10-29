namespace VotingSystem.Domain.Features.Project;

public static class ProjectValidationExtensions
{
    public static bool IsValidProjectName(this string? name)
    {
        return !string.IsNullOrWhiteSpace(name) && name.Length <= 200;
    }

    public static bool IsValidEventId(this long eventId)
    {
        return eventId > 0;
    }

    public static bool IsValidProjectTypeId(this int projectTypeId)
    {
        return projectTypeId > 0;
    }
}
