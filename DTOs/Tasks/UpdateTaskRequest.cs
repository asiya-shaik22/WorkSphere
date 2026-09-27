using WorkSphere.Enums;
using TaskStatus = WorkSphere.Enums.TaskStatus;


namespace WorkSphere.DTOs.Tasks
{
    public class UpdateTaskRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public TaskType Type { get; set; }
        public TaskPriority Priority { get; set; }
        public TaskStatus Status { get; set; }
        public int? AssignedToId { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
