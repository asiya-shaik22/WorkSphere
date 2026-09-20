using WorkSphere.Enums;

namespace WorkSphere.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        // Hashed password for authentication
        public string PasswordHash { get; set; } = string.Empty;

        // User role for authorization
        public UserRole Role { get; set; } = UserRole.Employee;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        // Projects created by this user
        public ICollection<Project> CreatedProjects { get; set; }
            = new List<Project>();


        // Tasks assigned to this user
        public ICollection<ProjectTask> AssignedTasks { get; set; }
            = new List<ProjectTask>();


        // Refresh tokens belonging to this user
        public ICollection<RefreshToken> RefreshTokens { get; set; }
            = new List<RefreshToken>();


        // Password reset tokens belonging to this user
        public ICollection<PasswordResetToken> PasswordResetTokens { get; set; }
            = new List<PasswordResetToken>();
    }
}