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

        //Navigation property for the relationship with User
        public User CreatedBy { get; set; } = null!;

        public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();



    }
}
