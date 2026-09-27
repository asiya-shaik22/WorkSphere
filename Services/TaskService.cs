using Microsoft.EntityFrameworkCore;
using WorkSphere.Data;
using WorkSphere.DTOs.Tasks;
using WorkSphere.Models;
using WorkSphere.Services;

namespace WorkSphere.Services;

public class TaskService : ITaskService
{
    private readonly AppDbContext _context;

    public TaskService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TaskResponse> CreateTaskAsync(CreateTaskRequest request)
    {
        // Project must exist
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == request.ProjectId);

        if (project == null)
            throw new InvalidOperationException(
                "Project not found.");

        // Check creator exists 
        var creator = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.CreatedById);

        if (creator == null)
            throw new InvalidOperationException(
                "Task Creator not found.");
        if(!creator.IsActive)
            throw new InvalidOperationException(
                "Inactive user cannot create a task.");


        // check assigned user exists
        var assignedUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.AssignedToId);

        if(assignedUser == null)
            throw new InvalidOperationException("Assigned user not found.");

        // Assigned user must be active
        if (!assignedUser.IsActive)
            throw new InvalidOperationException(
                "Inactive user cannot be assigned a task.");

        // User must belong to the project
        var creatorisMember = await _context.ProjectMembers
            .AnyAsync(pm =>
                pm.ProjectId == request.ProjectId &&
                pm.UserId == request.AssignedToId);

        if (!creatorisMember)
            throw new InvalidOperationException(
                "User is not a member of this project.");

        var assignedUserisMember = await _context.ProjectMembers
            .AnyAsync(pm =>
                pm.ProjectId == request.ProjectId &&
                pm.UserId == request.AssignedToId);

        if (!assignedUserisMember)
            throw new InvalidOperationException(
                "Assigned user is not a member of this project.");


        // Due date validation
        if (request.DueDate.HasValue &&
            request.DueDate.Value.Date < DateTime.UtcNow.Date)
        {
            throw new InvalidOperationException(
                "Due date cannot be in the past.");
        }

        var task = new ProjectTask
        {
            Title = request.Title,
            Description = request.Description,
            TaskType = request.Type,
            Priority = request.Priority,
            Status = request.Status,
            ProjectId = request.ProjectId,
            CreatedById = request.CreatedById,
            AssignedToId = request.AssignedToId,
            DueDate = request.DueDate,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(task);

        await _context.SaveChangesAsync();

        return MapToResponse(task, assignedUser);
    }

    public async Task<IEnumerable<TaskResponse>> GetTasksAsync(
        int projectId)
    {
        var tasks = await _context.Tasks
            .AsNoTracking()
            .Include(t => t.AssignedTo)
            .Where(t => t.ProjectId == projectId)
            .ToListAsync();

        return tasks.Select(t =>
            MapToResponse(t, t.AssignedTo));
    }

    public async Task<TaskResponse?> GetTaskByIdAsync(int id)
    {
        var task = await _context.Tasks
            .AsNoTracking()
            .Include(t => t.AssignedTo)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
            return null;

        return MapToResponse(task, task.AssignedTo);
    }

    public async Task<TaskResponse?> UpdateTaskAsync(
        int id,
        UpdateTaskRequest request)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
            return null;

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.AssignedToId);

        if (user == null)
            throw new InvalidOperationException(
                "Assigned user not found.");

        if (!user.IsActive)
            throw new InvalidOperationException(
                "Inactive user cannot be assigned a task.");

        var isMember = await _context.ProjectMembers
            .AnyAsync(pm =>
                pm.ProjectId == task.ProjectId &&
                pm.UserId == request.AssignedToId);

        if (!isMember)
            throw new InvalidOperationException(
                "User is not a member of this project.");

        if (request.DueDate.HasValue &&
            request.DueDate.Value.Date < DateTime.UtcNow.Date)
        {
            throw new InvalidOperationException(
                "Due date cannot be in the past.");
        }

        task.Title = request.Title;
        task.Description = request.Description;
        task.TaskType = request.Type;
        task.Priority = request.Priority;
        task.Status = request.Status;
        task.AssignedToId = request.AssignedToId;
        task.DueDate = request.DueDate;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToResponse(task, user);
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
            return false;

        _context.Tasks.Remove(task);

        await _context.SaveChangesAsync();

        return true;
    }

    private static TaskResponse MapToResponse(
        ProjectTask task,
        User? user)
    {
        return new TaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Type = task.TaskType,
            Priority = task.Priority,
            Status = task.Status,
            ProjectId = task.ProjectId,
            CreatedById = task.CreatedById,
            CreatedByName = task.CreatedBy?.Name ?? "Unknown",
            AssignedToId = task.AssignedToId,
            AssignedToName = user? .Name ?? "Unassigned",
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
}


