using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkSphere.Data;
using WorkSphere.Enums;
using WorkSphere.Models;

namespace WorkSphere.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // POST: api/tasks
        // Create a new task
        // =========================================================
        [HttpPost]
        public async Task<IActionResult> CreateTask(TaskCreateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest("Task title is required.");
            }

            // Check project
            var projectExists = await _context.Projects
                .AnyAsync(p => p.Id == request.ProjectId);

            if (!projectExists)
            {
                return BadRequest("Project does not exist.");
            }

            // Check creator
            var creatorExists = await _context.Users
                .AnyAsync(u => u.Id == request.CreatedById);

            if (!creatorExists)
            {
                return BadRequest("Task creator does not exist.");
            }

            // If task is assigned, make sure the user belongs to the project
            if (request.AssignedToId.HasValue)
            {
                var isProjectMember = await _context.ProjectMembers
                    .AnyAsync(pm =>
                        pm.ProjectId == request.ProjectId &&
                        pm.UserId == request.AssignedToId.Value);

                if (!isProjectMember)
                {
                    return BadRequest(
                        "The assigned user must be a member of the project.");
                }
            }

            var task = new ProjectTask
            {
                Title = request.Title,
                Description = request.Description,
                TaskType = request.TaskType,
                Status = request.Status,
                Priority = request.Priority,
                ProjectId = request.ProjectId,
                AssignedToId = request.AssignedToId,
                CreatedById = request.CreatedById,
                DueDate = request.DueDate,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(task);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetTaskById),
                new { id = task.Id },
                task);
        }


        // =========================================================
        // GET: api/tasks
        // View all tasks
        //
        // Optional filters:
        // ?projectId=1
        // ?status=InProgress
        // ?priority=High
        // ?assigneeId=2
        // ?taskType=Bug
        // ?search=login
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> GetTasks(
            int? projectId,
            WorkSphere.Enums.TaskStatus? status,
            TaskPriority? priority,
            int? assigneeId,
            TaskType? taskType,
            string? search)
        {
            var query = _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedTo)
                .Include(t => t.CreatedBy)
                .AsQueryable();

            // Filter by Project
            if (projectId.HasValue)
            {
                query = query.Where(t =>
                    t.ProjectId == projectId.Value);
            }

            // Filter by Status
            if (status.HasValue)
            {
                query = query.Where(t =>
                    t.Status == status.Value);
            }

            // Filter by Priority
            if (priority.HasValue)
            {
                query = query.Where(t =>
                    t.Priority == priority.Value);
            }

            // Filter by Assignee
            if (assigneeId.HasValue)
            {
                query = query.Where(t =>
                    t.AssignedToId == assigneeId.Value);
            }

            // Filter by Task Type
            if (taskType.HasValue)
            {
                query = query.Where(t =>
                    t.TaskType == taskType.Value);
            }

            // Search by task title
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(t =>
                    t.Title.Contains(search));
            }

            var tasks = await query
                .Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Description,
                    t.TaskType,
                    t.Status,
                    t.Priority,

                    t.ProjectId,
                    ProjectName = t.Project.Name,

                    t.AssignedToId,
                    AssignedTo = t.AssignedTo != null
                        ? t.AssignedTo.Name
                        : null,

                    t.CreatedById,
                    CreatedBy = t.CreatedBy.Name,

                    t.DueDate,
                    t.CreatedAt,
                    t.UpdatedAt
                })
                .ToListAsync();

            return Ok(tasks);
        }


        // =========================================================
        // GET: api/tasks/{id}
        // View a specific task
        // =========================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedTo)
                .Include(t => t.CreatedBy)
                .Where(t => t.Id == id)
                .Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Description,
                    t.TaskType,
                    t.Status,
                    t.Priority,

                    t.ProjectId,
                    ProjectName = t.Project.Name,

                    t.AssignedToId,
                    AssignedTo = t.AssignedTo != null
                        ? t.AssignedTo.Name
                        : null,

                    t.CreatedById,
                    CreatedBy = t.CreatedBy.Name,

                    t.DueDate,
                    t.CreatedAt,
                    t.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (task == null)
            {
                return NotFound("Task not found.");
            }

            return Ok(task);
        }


        // =========================================================
        // PUT: api/tasks/{id}
        // Update task
        // =========================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(
            int id,
            TaskUpdateRequest request)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound("Task not found.");
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest("Task title is required.");
            }

            // If assigning/reassigning a user,
            // verify that the user is a project member
            if (request.AssignedToId.HasValue)
            {
                var isProjectMember = await _context.ProjectMembers
                    .AnyAsync(pm =>
                        pm.ProjectId == task.ProjectId &&
                        pm.UserId == request.AssignedToId.Value);

                if (!isProjectMember)
                {
                    return BadRequest(
                        "The assigned user must be a member of the project.");
                }
            }

            task.Title = request.Title;
            task.Description = request.Description;
            task.TaskType = request.TaskType;
            task.Priority = request.Priority;
            task.AssignedToId = request.AssignedToId;
            task.DueDate = request.DueDate;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(task);
        }


        // =========================================================
        // PATCH: api/tasks/{id}/status
        // Change task status
        // =========================================================
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeTaskStatus(
            int id,
            ChangeTaskStatusRequest request)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound("Task not found.");
            }

            task.Status = request.Status;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Task status updated successfully.",
                task.Id,
                task.Status
            });
        }


        // =========================================================
        // DELETE: api/tasks/{id}
        // Delete/archive task
        // =========================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound("Task not found.");
            }

            _context.Tasks.Remove(task);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Task deleted successfully."
            });
        }
    }


    // =============================================================
    // REQUEST MODELS
    // =============================================================

    public class TaskCreateRequest
    {
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public TaskType TaskType { get; set; } = TaskType.Task;

        public WorkSphere.Enums.TaskStatus Status { get; set; }
            = WorkSphere.Enums.TaskStatus.ToDo;

        public TaskPriority Priority { get; set; }
            = TaskPriority.Medium;

        public int ProjectId { get; set; }

        public int? AssignedToId { get; set; }

        public int CreatedById { get; set; }

        public DateTime? DueDate { get; set; }
    }


    public class TaskUpdateRequest
    {
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public TaskType TaskType { get; set; } = TaskType.Task;

        public TaskPriority Priority { get; set; }
            = TaskPriority.Medium;

        public int? AssignedToId { get; set; }

        public DateTime? DueDate { get; set; }
    }


    public class ChangeTaskStatusRequest
    {
        public WorkSphere.Enums.TaskStatus Status { get; set; }
    }
}