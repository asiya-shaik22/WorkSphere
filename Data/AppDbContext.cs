using Microsoft.EntityFrameworkCore;
using WorkSphere.Models;

namespace WorkSphere.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<ProjectTask> Tasks { get; set; }

        public DbSet<ProjectMember> ProjectMembers { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // USER
            // =========================

            // Email must be unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();


            // =========================
            // PROJECT
            // =========================

            // Project - Created By User
            modelBuilder.Entity<Project>()
                .HasOne(p => p.CreatedBy)
                .WithMany(u => u.CreatedProjects)
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);


            // =========================
            // PROJECT MEMBER
            // =========================

            // Project - Members
            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.Project)
                .WithMany()
                .HasForeignKey(pm => pm.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - Project Membership
            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.User)
                .WithMany()
                .HasForeignKey(pm => pm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Prevent duplicate membership
            modelBuilder.Entity<ProjectMember>()
                .HasIndex(pm => new { pm.ProjectId, pm.UserId })
                .IsUnique();


            // =========================
            // TASK
            // =========================

            // Project - Tasks
            modelBuilder.Entity<ProjectTask>()
                .HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Task - Assigned User
            modelBuilder.Entity<ProjectTask>()
                .HasOne(t => t.AssignedTo)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.AssignedToId)
                .OnDelete(DeleteBehavior.SetNull);

            // Task - Created By User
            modelBuilder.Entity<ProjectTask>()
                .HasOne(t => t.CreatedBy)
                .WithMany(u => u.CreatedTasks)
                .HasForeignKey(t => t.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);


            // =========================
            // COMMENT
            // =========================

            // Task - Comments
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Task)
                .WithMany()
                .HasForeignKey(c => c.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - Comments
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            // =========================
            // REFRESH TOKEN
            // =========================

            modelBuilder.Entity<RefreshToken>()
                .HasOne(r => r.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // =========================
            // PASSWORD RESET TOKEN
            // =========================

            modelBuilder.Entity<PasswordResetToken>()
                .HasOne(p => p.User)
                .WithMany(u => u.PasswordResetTokens)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}