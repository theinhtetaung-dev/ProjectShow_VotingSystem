using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VotingSystem.Console.MemoryData;
using VotingSystem.Console.Models;

namespace VotingSystem.Console.Features.Projects;

public class ProjectService : IProjectService
{
    public Project CreateProject(string name, string description)
    {
        var project = new Project
        {
            Id = Data.MaxProjId + 1,
            Name = name,
            Description = description,
            VoteCount = 0
        };
        Data.ProjectsData.Add(project);
        return project;

    }

    public Project? UpdateProject(int projectId, string name, string description)
    {
        var project = Data.ProjectsData.FirstOrDefault(p => p.Id == projectId);
        if (project != null)
        {
            project.Name = name;
            project.Description = description;
        }
        return project;
    }
    public bool DeleteProject(int projectId)
    {
        var project = Data.ProjectsData.FirstOrDefault(p => p.Id == projectId);
        if (project != null)
        {
            Data.ProjectsData.Remove(project);
            return true;
        }
        return false;
    }

    public Project? GetProject(int projectId)
    {
        if (Data.ProjectsData == null || Data.ProjectsData.Count == 0)
        {
            return null;
        }
        return Data.ProjectsData.FirstOrDefault(p => p.Id == projectId);
    }

    public List<Project>? GetProjects()
    {
        if (Data.ProjectsData == null || Data.ProjectsData.Count == 0)
        {
            return null;
        }
        return Data.ProjectsData;
    }
}


