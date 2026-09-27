using WorkSphere.Enums;
using TaskStatus = WorkSphere.Enums.TaskStatus;

namespace WorkSphere.DTOs.Tasks
{
    public class CreateTaskRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public TaskType Type { get; set; } = TaskType.Task;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public TaskStatus Status { get; set; } = TaskStatus.ToDo;

        public int ProjectId { get; set; }

        public int CreatedById { get; set; }
        public int? AssignedToId { get; set; }

        public DateTime? DueDate { get; set; }

    }
}
