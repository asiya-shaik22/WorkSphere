using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkSphere.Data;
using WorkSphere.Enums;
using WorkSphere.Models;

namespace WorkSphere.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectsController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/projects
        // Create a new project
        [HttpPost]
        public async Task<IActionResult> CreateProject(ProjectCreateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("Project name is required.");
            }

            var userExists = await _context.Users
                .AnyAsync(u => u.Id == request.CreatedById);

            if (!userExists)
            {
                return BadRequest("Project creator does not exist.");
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

            return CreatedAtAction(
                nameof(GetProjectById),
                new { id = project.Id },
                project);
        }

        // GET: api/projects
        // Get all projects
        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var projects = await _context.Projects
                .Include(p => p.CreatedBy)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Status,
                    p.StartDate,
                    p.EndDate,
                    p.CreatedAt,
                    p.UpdatedAt,
                    CreatedById = p.CreatedById,
                    CreatedBy = p.CreatedBy.Name
                })
                .ToListAsync();

            return Ok(projects);
        }

        // GET: api/projects/{id}
        // Get a specific project
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            var project = await _context.Projects
                .Include(p => p.CreatedBy)
                .Include(p => p.Tasks)
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Status,
                    p.StartDate,
                    p.EndDate,
                    p.CreatedAt,
                    p.UpdatedAt,
                    CreatedById = p.CreatedById,
                    CreatedBy = p.CreatedBy.Name,
                    TaskCount = p.Tasks.Count
                })
                .FirstOrDefaultAsync();

            if (project == null)
            {
                return NotFound("Project not found.");
            }

            return Ok(project);
        }

        // PUT: api/projects/{id}
        // Update a project
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(
            int id,
            ProjectUpdateRequest request)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound("Project not found.");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("Project name is required.");
            }

            var userExists = await _context.Users
                .AnyAsync(u => u.Id == request.CreatedById);

            if (!userExists)
            {
                return BadRequest("Project creator does not exist.");
            }

            project.Name = request.Name;
            project.Description = request.Description;
            project.Status = request.Status;
            project.StartDate = request.StartDate;
            project.EndDate = request.EndDate;
            project.CreatedById = request.CreatedById;
            project.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(project);
        }

        // DELETE: api/projects/{id}
        // Archive a project instead of permanently deleting it
        [HttpDelete("{id}")]
        public async Task<IActionResult> ArchiveProject(int id)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound("Project not found.");
            }

            project.Status = ProjectStatus.Archived;
            project.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Project archived successfully.",
                project.Id,
                project.Name,
                project.Status
            });
        }
    }

    public class ProjectCreateRequest
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ProjectStatus Status { get; set; } = ProjectStatus.Planned;

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int CreatedById { get; set; }
    }

    public class ProjectUpdateRequest
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ProjectStatus Status { get; set; } = ProjectStatus.Planned;

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int CreatedById { get; set; }
    }
}