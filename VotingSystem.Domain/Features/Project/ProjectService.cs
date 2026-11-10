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
    public async Task<Result<PageResult<ProjectResponseModel>>> GetProjectlist(
        int pageNumber,
        int pageSize,
        string? status)
    {
        try
        {
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 1, 100);

            Enums.EventStatus? eventStatus = null;
            if (!string.IsNullOrWhiteSpace(status))
            {
                eventStatus = status.ToEventStatus();
                if (!eventStatus.IsValidEventStatus())
                    return Result<PageResult<ProjectResponseModel>>.ValidationError(
                        "Status must be Upcoming, Active, Ended, or Published.");
            }

            var query = _dbContext.TblProjects
                .AsNoTracking()
                .Where(project => !project.DeleteFlag);

            if (eventStatus.HasValue)
            {
                var statusValue = eventStatus.Value.ToString();
                query = query.Where(project => project.Event.Status == statusValue);
            }

            var totalCount = await query.CountAsync();
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
            pageNumber = Math.Min(pageNumber, totalPages);

            var projects = await query
                .OrderBy(project => project.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(project => new ProjectResponseModel
                {
                    Id = project.Id,
                    EventId = project.EventId,
                    ProjectTypeId = project.ProjectTypeId,
                    EventCode = project.Event.Code,
                    ProjectTypeName = project.ProjectType.Name,
                    Status = project.Event.Status,
                    Name = project.Name,
                    Description = project.Description,
                    VoteCounts = project.VoteCounts
                })
                .ToListAsync();

            var pageResult = new PageResult<ProjectResponseModel>
            {
                Items = projects,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            var message = totalCount == 0 ? "No projects found." : "Success";
            return Result<PageResult<ProjectResponseModel>>.Success(pageResult, message);
        }
        catch (Exception ex)
        {
            return Result<PageResult<ProjectResponseModel>>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Get Project By ID
    public async Task<Result<ProjectResponseModel>> GetProjectByID(long id)
    {
        try
        {
            var project = await GetResponseModelAsync(id);
            if (project == null)
            {
                return Result<ProjectResponseModel>.NotFound("Item Not Found!");
            }

            return Result<ProjectResponseModel>.Success(project, "Success");
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
            if (!request.Name.IsValidRequiredString(200))
            {
                return Result<ProjectResponseModel>.ValidationError("Name is required and must not exceed 200 characters.");
            }

            if (!request.Description.IsValidOptionalString(1000))
            {
                return Result<ProjectResponseModel>.ValidationError("Description must not exceed 1000 characters.");
            }

            if (!request.EventId.IsPositive())
            {
                return Result<ProjectResponseModel>.ValidationError("Event ID must be greater than zero.");
            }

            if (!request.ProjectTypeId.IsPositive())
            {
                return Result<ProjectResponseModel>.ValidationError("Project type ID must be greater than zero.");
            }

            var eventExists = await _dbContext.TblEvents
                .AnyAsync(x => x.Id == request.EventId && !x.DeleteFlag);
            if (!eventExists)
            {
                return Result<ProjectResponseModel>.ValidationError("Event not found.");
            }

            var projectTypeExists = await _dbContext.TblProjectTypes
                .AnyAsync(x => x.Id == request.ProjectTypeId && !x.DeleteFlag);
            if (!projectTypeExists)
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

            var resModel = await GetResponseModelAsync(newProject.Id);
            if (resModel == null)
                return Result<ProjectResponseModel>.SystemError("Unable to load the created project.");

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

            if (request.ExpectedEventId.HasValue && project.EventId != request.ExpectedEventId.Value)
            {
                return Result<ProjectResponseModel>.ValidationError("Project does not belong to the selected event.");
            }

            if (request.Name != null && !request.Name.IsValidRequiredString(200))
            {
                return Result<ProjectResponseModel>.ValidationError("Name is required and must not exceed 200 characters.");
            }

            if (request.Description != null && !request.Description.IsValidOptionalString(1000))
            {
                return Result<ProjectResponseModel>.ValidationError("Description must not exceed 1000 characters.");
            }

            if (request.EventId.HasValue && !request.EventId.Value.IsPositive())
            {
                return Result<ProjectResponseModel>.ValidationError("Event ID must be greater than zero.");
            }

            if (request.ProjectTypeId.HasValue && !request.ProjectTypeId.Value.IsPositive())
            {
                return Result<ProjectResponseModel>.ValidationError("Project type ID must be greater than zero.");
            }

            var eventExists = await _dbContext.TblEvents
                .AnyAsync(x => x.Id == request.EventId && !x.DeleteFlag);
            if (request.EventId.HasValue && !eventExists)
            {
                return Result<ProjectResponseModel>.ValidationError("Event not found.");
            }

            var projectTypeExists = await _dbContext.TblProjectTypes
                .AnyAsync(x => x.Id == request.ProjectTypeId && !x.DeleteFlag);
            if (request.ProjectTypeId.HasValue && !projectTypeExists)
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

            var resModel = await GetResponseModelAsync(project.Id);
            if (resModel == null)
                return Result<ProjectResponseModel>.SystemError("Unable to load the updated project.");

            return Result<ProjectResponseModel>.Success(resModel, "Project updated successfully.");
        }
        catch (Exception ex)
        {
            return Result<ProjectResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Delete Project
    public async Task<Result<ProjectResponseModel>> DeleteProject(long id, long? eventId = null)
    {
        try
        {
            var project = await _dbContext.TblProjects
                .FirstOrDefaultAsync(x => x.Id == id && !x.DeleteFlag);
            if (project == null)
            {
                return Result<ProjectResponseModel>.NotFound("Item Not Found!");
            }

            if (eventId.HasValue && project.EventId != eventId.Value)
            {
                return Result<ProjectResponseModel>.ValidationError("Project does not belong to the selected event.");
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

    private Task<ProjectResponseModel?> GetResponseModelAsync(long id)
    {
        return _dbContext.TblProjects
            .AsNoTracking()
            .Where(project => project.Id == id && !project.DeleteFlag)
            .Select(project => new ProjectResponseModel
            {
                Id = project.Id,
                EventId = project.EventId,
                ProjectTypeId = project.ProjectTypeId,
                EventCode = project.Event.Code,
                ProjectTypeName = project.ProjectType.Name,
                Status = project.Event.Status,
                Name = project.Name,
                Description = project.Description,
                VoteCounts = project.VoteCounts
            })
            .FirstOrDefaultAsync();
    }
}
