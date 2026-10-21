using VotingSystem.Console.Models;

namespace VotingSystem.Console.Features.Projects
{
    public interface IProjectService
    {
        Project CreateProject(string name, string description);
        bool DeleteProject(int projectId);
        Project? GetProject(int projectId);
        List<Project>? GetProjects();
        Project? UpdateProject(int projectId, string name, string description);
    }
}