using WorkSphere.Enums;
using TaskStatus = WorkSphere.Enums.TaskStatus;

namespace WorkSphere.DTOs.Tasks
{
    public class TaskResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskType Type { get; set; }
        public TaskPriority Priority { get; set; }
        public TaskStatus Status { get; set; }
        public int ProjectId { get; set; }
        public int? AssignedToId { get; set; }
        public string AssignedToName { get; set; } = string.Empty;  
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public int CreatedById { get; set; }
        public string CreatedByName { get; set; } = string.Empty;   
        public DateTime? UpdatedAt { get; set; }
    }
}
