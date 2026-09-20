using WorkSphere.Enums;

namespace WorkSphere.Models
{
    public class Project
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ProjectStatus Status { get; set; } = ProjectStatus.Planned;

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Foreign Key - User who created the project
        public int CreatedById { get; set; }

        // Navigation property
        public User CreatedBy { get; set; } = null!;

        // Navigation property - Tasks belonging to this project
        public ICollection<ProjectTask> Tasks { get; set; }
            = new List<ProjectTask>();
    }
}