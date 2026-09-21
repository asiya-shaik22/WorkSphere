using WorkSphere.Enums;

namespace WorkSphere.Models
{
    public class ProjectTask
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public TaskType TaskType { get; set; } = TaskType.Task;

        public WorkSphere.Enums.TaskStatus Status { get; set; }
            = WorkSphere.Enums.TaskStatus.ToDo;

        public TaskPriority Priority { get; set; }
            = TaskPriority.Medium;

        public DateTime? DueDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public int ProjectId { get; set; }

        public Project Project { get; set; } = null!;

        public int? AssignedToId { get; set; }

        public User? AssignedTo { get; set; }

        public int CreatedById { get; set; }

        public User CreatedBy { get; set; } = null!;
    }
}