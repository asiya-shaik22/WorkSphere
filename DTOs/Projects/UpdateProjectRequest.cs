using WorkSphere.Enums;

namespace WorkSphere.DTOs.Projects
{
    public class UpdateProjectRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        
        public ProjectStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}
