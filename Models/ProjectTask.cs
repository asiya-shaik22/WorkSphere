using WorkSphere.Enums;

namespace WorkSphere.Models
{
    public class ProjectTask
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public WorkSphere.Enums.TaskStatus Status { get; set; }
            = WorkSphere.Enums.TaskStatus.ToDo;

        public TaskPriority Priority { get; set; }
            = TaskPriority.Medium;

        public DateTime? DueDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }


        // Foreign Key - Project
        public int ProjectId { get; set; }

        // Navigation property
        public Project Project { get; set; } = null!;


        // Foreign Key - User assigned to the task
        public int? AssignedToId { get; set; }

        // Navigation property
        public User? AssignedTo { get; set; }
    }
}