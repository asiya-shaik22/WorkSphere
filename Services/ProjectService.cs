using Microsoft.EntityFrameworkCore;
using WorkSphere.Data;
using WorkSphere.DTOs.Projects;
using WorkSphere.Models;

namespace WorkSphere.Services
{
    public class ProjectService : IProjectService
    {
        private readonly AppDbContext _context;

        public ProjectService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProjectResponse> CreateProjectAsync(CreateProjectRequest request)
        {

            if (request.EndDate.HasValue &&
                request.EndDate.Value <= request.StartDate)
            {
                throw new ArgumentException("End date must be after start date.");
            }
            var creator = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.CreatedById);
            if (creator == null)
            {
                throw new ArgumentException("Project creator does not exist.");
            }
            if (!creator.IsActive) // assumes User has a bool IsActive property
            {
                throw new InvalidOperationException("Project creator is not active.");
            }

            var project = new Project       
            {
                Name = request.Name,
                Description = request.Description,
                Status = request.Status,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                CreatedById = request.CreatedById,
                CreatedAt = DateTime.UtcNow
            };
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return MapToProjectResponse(project);
        }

        public async Task<IEnumerable<ProjectResponse>> GetProjectsAsync()
        {
            var projects = await _context.Projects.AsNoTracking().ToListAsync();
            return projects.Select(MapToProjectResponse);
        }

        public async Task<ProjectResponse?> GetProjectByIdAsync(int id)
        {
            var project = await _context.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            return project != null ? MapToProjectResponse(project) : null;
        }
        public async Task<ProjectResponse?> UpdateProjectAsync(int id, UpdateProjectRequest request)
        {

            if (request.EndDate.HasValue &&
                request.EndDate.Value <= request.StartDate)
            {
                throw new ArgumentException("End date must be after start date.");
            }

            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);
            if (project == null)
            {
                return null;
            }
            project.Name = request.Name;
            project.Description = request.Description;
            project.Status = request.Status;
            project.StartDate = request.StartDate;
            project.EndDate = request.EndDate;
            project.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return MapToProjectResponse(project);


        }
        public async Task<bool> DeleteProjectAsync(int id)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);
            if (project == null)
            {
                return false;
            }

            project.Status = Enums.ProjectStatus.Archived;
            project.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        private static ProjectResponse MapToProjectResponse(Project project)
        {
            return new ProjectResponse
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Status = project.Status,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                CreatedById = project.CreatedById,
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt
            };
        }

    }
}
