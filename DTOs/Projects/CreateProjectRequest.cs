using WorkSphere.Enums;

namespace WorkSphere.DTOs.Projects
{
    public class CreateProjectRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ProjectStatus Status { get; set; } = ProjectStatus.Planned;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int CreatedById { get; set; }
    }
}
