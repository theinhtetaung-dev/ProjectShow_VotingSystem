using Microsoft.EntityFrameworkCore;
using VotingSystem.DataAccess.AppDbContextModels;
using VotingSystem.Shared;

namespace VotingSystem.Domain.Features.Project;

public class ProjectService : IProjectService
{
    private readonly AppDbContext _dbContext;

    public ProjectService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region Get Project List
    public async Task<Result<List<ProjectResponseModel>>> GetProjectlist()
    {
        try
        {
            var projects = await _dbContext.TblProjects
                .Where(x => !x.DeleteFlag)
                .ToListAsync();
            var resModel = new List<ProjectResponseModel>();

            foreach (var project in projects)
            {
                resModel.Add(new ProjectResponseModel
                {
                    Id = project.Id,
                    EventId = project.EventId,
                    ProjectTypeId = project.ProjectTypeId,
                    Name = project.Name,
                    Description = project.Description,
                    VoteCounts = project.VoteCounts
                });
            }

            string message = projects.Count == 0 ? "No projects found." : "Success";
            return Result<List<ProjectResponseModel>>.Success(resModel, message);
        }
        catch (Exception ex)
        {
            return Result<List<ProjectResponseModel>>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Get Project By ID
    public async Task<Result<ProjectResponseModel>> GetProjectByID(long id)
    {
        try
        {
            var project = await _dbContext.TblProjects
                .FirstOrDefaultAsync(x => x.Id == id && !x.DeleteFlag);
            if (project == null)
            {
                return Result<ProjectResponseModel>.NotFound("Item Not Found!");
            }

            var resModel = new ProjectResponseModel
            {
                Id = project.Id,
                EventId = project.EventId,
                ProjectTypeId = project.ProjectTypeId,
                Name = project.Name,
                Description = project.Description,
                VoteCounts = project.VoteCounts
            };
            return Result<ProjectResponseModel>.Success(resModel, "Success");
        }
        catch (Exception ex)
        {
            return Result<ProjectResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Create Project
    public async Task<Result<ProjectResponseModel>> CreateProjectAsync(ProjectCreateRequestModel request)
    {
        try
        {
            if (!request.Name.IsValidProjectName())
            {
                return Result<ProjectResponseModel>.ValidationError("Name is required and must not exceed 200 characters.");
            }

            if (!request.Description.IsValidDescription())
            {
                return Result<ProjectResponseModel>.ValidationError("Description must not exceed 1000 characters.");
            }

            if (!request.EventId.IsValidEventId())
            {
                return Result<ProjectResponseModel>.ValidationError("Event ID must be greater than zero.");
            }

            if (!request.ProjectTypeId.IsValidProjectTypeId())
            {
                return Result<ProjectResponseModel>.ValidationError("Project type ID must be greater than zero.");
            }

            if (!await _dbContext.TblEvents.AnyAsync(x => x.Id == request.EventId && !x.DeleteFlag))
            {
                return Result<ProjectResponseModel>.ValidationError("Event not found.");
            }

            if (!await _dbContext.TblProjectTypes.AnyAsync(x => x.Id == request.ProjectTypeId && !x.DeleteFlag))
            {
                return Result<ProjectResponseModel>.ValidationError("Project type not found.");
            }

            var newProject = new TblProject
            {
                EventId = request.EventId,
                ProjectTypeId = request.ProjectTypeId,
                Name = request.Name,
                Description = request.Description,
                VoteCounts = 0,
                IsActive = true,
                DeleteFlag = false,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.TblProjects.Add(newProject);
            await _dbContext.SaveChangesAsync();

            var resModel = new ProjectResponseModel
            {
                Id = newProject.Id,
                EventId = newProject.EventId,
                ProjectTypeId = newProject.ProjectTypeId,
                Name = newProject.Name,
                Description = newProject.Description,
                VoteCounts = newProject.VoteCounts
            };
            return Result<ProjectResponseModel>.Success(resModel, "Project created successfully.");
        }
        catch (Exception ex)
        {
            return Result<ProjectResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Update Project
    public async Task<Result<ProjectResponseModel>> UpdateProject(ProjectUpdateRequestModel request)
    {
        try
        {
            var project = await _dbContext.TblProjects
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.DeleteFlag);
            if (project == null)
            {
                return Result<ProjectResponseModel>.NotFound("Item Not Found!");
            }

            if (request.Name != null && !request.Name.IsValidProjectName())
            {
                return Result<ProjectResponseModel>.ValidationError("Name is required and must not exceed 200 characters.");
            }

            if (request.Description != null && !request.Description.IsValidDescription())
            {
                return Result<ProjectResponseModel>.ValidationError("Description must not exceed 1000 characters.");
            }

            if (request.EventId.HasValue && !request.EventId.Value.IsValidEventId())
            {
                return Result<ProjectResponseModel>.ValidationError("Event ID must be greater than zero.");
            }

            if (request.ProjectTypeId.HasValue && !request.ProjectTypeId.Value.IsValidProjectTypeId())
            {
                return Result<ProjectResponseModel>.ValidationError("Project type ID must be greater than zero.");
            }

            if (request.EventId.HasValue
                && !await _dbContext.TblEvents.AnyAsync(x => x.Id == request.EventId.Value && !x.DeleteFlag))
            {
                return Result<ProjectResponseModel>.ValidationError("Event not found.");
            }

            if (request.ProjectTypeId.HasValue
                && !await _dbContext.TblProjectTypes.AnyAsync(x => x.Id == request.ProjectTypeId.Value && !x.DeleteFlag))
            {
                return Result<ProjectResponseModel>.ValidationError("Project type not found.");
            }

            if (request.EventId.HasValue)
            {
                project.EventId = request.EventId.Value;
            }

            if (request.ProjectTypeId.HasValue)
            {
                project.ProjectTypeId = request.ProjectTypeId.Value;
            }

            if (request.Name != null)
            {
                project.Name = request.Name;
            }

            if (request.Description != null)
            {
                project.Description = request.Description;
            }

            var hasUpdates = request.EventId.HasValue
                || request.ProjectTypeId.HasValue
                || request.Name != null
                || request.Description != null;
            if (hasUpdates)
            {
                project.ModifiedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }

            var resModel = new ProjectResponseModel
            {
                Id = project.Id,
                EventId = project.EventId,
                ProjectTypeId = project.ProjectTypeId,
                Name = project.Name,
                Description = project.Description,
                VoteCounts = project.VoteCounts
            };
            return Result<ProjectResponseModel>.Success(resModel, "Project updated successfully.");
        }
        catch (Exception ex)
        {
            return Result<ProjectResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Delete Project
    public async Task<Result<ProjectResponseModel>> DeleteProject(long id)
    {
        try
        {
            var project = await _dbContext.TblProjects
                .FirstOrDefaultAsync(x => x.Id == id && !x.DeleteFlag);
            if (project == null)
            {
                return Result<ProjectResponseModel>.NotFound("Item Not Found!");
            }

            project.DeleteFlag = true;
            project.IsActive = false;
            project.ModifiedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return Result<ProjectResponseModel>.DeleteSuccess("Project deleted successfully.");
        }
        catch (Exception ex)
        {
            return Result<ProjectResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion
}
