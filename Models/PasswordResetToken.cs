namespace WorkSphere.Models
{
    public class PasswordResetToken
    {
        public int Id { get; set; }

        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public bool IsUsed { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key - User
        public int UserId { get; set; }

        // Navigation property
        public User User { get; set; } = null!;
    }
}