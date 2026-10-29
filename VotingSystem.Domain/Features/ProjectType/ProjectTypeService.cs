using Microsoft.EntityFrameworkCore;
using VotingSystem.DataAccess.AppDbContextModels;
using VotingSystem.Shared;

namespace VotingSystem.Domain.Features.ProjectType;

public class ProjectTypeService : IProjectTypeService
{
    private readonly AppDbContext _dbContext;

    public ProjectTypeService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region Get Project Type List
    public async Task<Result<List<ProjectTypeResponseModel>>> GetProjectTypelist()
    {
        try
        {
            var projectTypes = await _dbContext.TblProjectTypes
                .Where(x => !x.DeleteFlag)
                .ToListAsync();
            var resModel = new List<ProjectTypeResponseModel>();

            foreach (var projectType in projectTypes)
            {
                resModel.Add(new ProjectTypeResponseModel
                {
                    Id = projectType.Id,
                    Name = projectType.Name,
                    Code = projectType.Code
                });
            }

            string message = projectTypes.Count == 0 ? "No project types found." : "Success";
            return Result<List<ProjectTypeResponseModel>>.Success(resModel, message);
        }
        catch (Exception ex)
        {
            return Result<List<ProjectTypeResponseModel>>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Get Project Type By ID
    public async Task<Result<ProjectTypeResponseModel>> GetProjectTypeByID(int id)
    {
        try
        {
            var projectType = await _dbContext.TblProjectTypes
                .FirstOrDefaultAsync(x => x.Id == id && !x.DeleteFlag);
            if (projectType == null)
            {
                return Result<ProjectTypeResponseModel>.NotFound("Item Not Found!");
            }

            var resModel = new ProjectTypeResponseModel
            {
                Id = projectType.Id,
                Name = projectType.Name,
                Code = projectType.Code
            };
            return Result<ProjectTypeResponseModel>.Success(resModel, "Success");
        }
        catch (Exception ex)
        {
            return Result<ProjectTypeResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Create Project Type
    public async Task<Result<ProjectTypeResponseModel>> CreateProjectTypeAsync(ProjectTypeCreateRequestModel request)
    {
        try
        {
            if (!request.Name.IsValidRequiredString(50))
            {
                return Result<ProjectTypeResponseModel>.ValidationError("Name is required and must not exceed 50 characters.");
            }

            if (!request.Code.IsValidRequiredString(50))
            {
                return Result<ProjectTypeResponseModel>.ValidationError("Code is required and must not exceed 50 characters.");
            }

            var nameExists = await _dbContext.TblProjectTypes
                .AnyAsync(x => x.Name == request.Name);
            if (nameExists)
            {
                return Result<ProjectTypeResponseModel>.ValidationError("Project type name already exists.");
            }

            var codeExists = await _dbContext.TblProjectTypes
                .AnyAsync(x => x.Code == request.Code);
            if (codeExists)
            {
                return Result<ProjectTypeResponseModel>.ValidationError("Project type code already exists.");
            }

            var newProjectType = new TblProjectType
            {
                Name = request.Name,
                Code = request.Code,
                IsActive = true,
                DeleteFlag = false
            };
            _dbContext.TblProjectTypes.Add(newProjectType);
            await _dbContext.SaveChangesAsync();

            var resModel = new ProjectTypeResponseModel
            {
                Id = newProjectType.Id,
                Name = newProjectType.Name,
                Code = newProjectType.Code
            };
            return Result<ProjectTypeResponseModel>.Success(resModel, "Project type created successfully.");
        }
        catch (Exception ex)
        {
            return Result<ProjectTypeResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Update Project Type
    public async Task<Result<ProjectTypeResponseModel>> UpdateProjectType(ProjectTypeUpdateRequestModel request)
    {
        try
        {
            var projectType = await _dbContext.TblProjectTypes
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.DeleteFlag);
            if (projectType == null)
            {
                return Result<ProjectTypeResponseModel>.NotFound("Item Not Found!");
            }

            if (request.Name != null && !request.Name.IsValidRequiredString(50))
            {
                return Result<ProjectTypeResponseModel>.ValidationError("Name is required and must not exceed 50 characters.");
            }

            if (request.Code != null && !request.Code.IsValidRequiredString(50))
            {
                return Result<ProjectTypeResponseModel>.ValidationError("Code is required and must not exceed 50 characters.");
            }

            var nameExists = await _dbContext.TblProjectTypes
                .AnyAsync(x => x.Name == request.Name && x.Id != request.Id);
            if (request.Name != null && nameExists)
            {
                return Result<ProjectTypeResponseModel>.ValidationError("Project type name already exists.");
            }

            var codeExists = await _dbContext.TblProjectTypes
                .AnyAsync(x => x.Code == request.Code && x.Id != request.Id);
            if (request.Code != null && codeExists)
            {
                return Result<ProjectTypeResponseModel>.ValidationError("Project type code already exists.");
            }

            if (request.Name != null)
            {
                projectType.Name = request.Name;
            }

            if (request.Code != null)
            {
                projectType.Code = request.Code;
            }

            var hasUpdates = request.Name != null || request.Code != null;
            if (hasUpdates)
            {
                await _dbContext.SaveChangesAsync();
            }

            var resModel = new ProjectTypeResponseModel
            {
                Id = projectType.Id,
                Name = projectType.Name,
                Code = projectType.Code
            };
            return Result<ProjectTypeResponseModel>.Success(resModel, "Project type updated successfully.");
        }
        catch (Exception ex)
        {
            return Result<ProjectTypeResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion

    #region Delete Project Type
    public async Task<Result<ProjectTypeResponseModel>> DeleteProjectType(int id)
    {
        try
        {
            var projectType = await _dbContext.TblProjectTypes
                .FirstOrDefaultAsync(x => x.Id == id && !x.DeleteFlag);
            if (projectType == null)
            {
                return Result<ProjectTypeResponseModel>.NotFound("Item Not Found!");
            }

            projectType.DeleteFlag = true;
            projectType.IsActive = false;
            await _dbContext.SaveChangesAsync();
            return Result<ProjectTypeResponseModel>.DeleteSuccess("Project type deleted successfully.");
        }
        catch (Exception ex)
        {
            return Result<ProjectTypeResponseModel>.SystemError(ex.Message);
        }
    }
    #endregion
}
