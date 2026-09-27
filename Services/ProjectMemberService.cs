using Microsoft.EntityFrameworkCore;
using WorkSphere.Data;
using WorkSphere.DTOs.ProjectMembers;
using WorkSphere.Models;


namespace WorkSphere.Services
{
    public class ProjectMemberService : IProjectMemberService
    {
        private readonly AppDbContext _context;

        public ProjectMemberService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProjectMemberResponse> AddMemberAsync(int projectId, AddProjectMemberRequest request)
        {
            var projectExists = await _context.Projects.AnyAsync(p => p.Id == projectId);

            if (!projectExists)
                throw new InvalidOperationException ("Project not found.");


            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId);
            if (user == null)
                throw new InvalidOperationException("User not found.");

            if(!user.IsActive)
                throw new InvalidOperationException("User is not active.");

            // Prevent duplicate membership
            var alreadyMember = await _context.ProjectMembers
                .AnyAsync(pm =>
                    pm.ProjectId == projectId &&
                    pm.UserId == request.UserId);
            if (alreadyMember)
                throw new InvalidOperationException("User is already a member of this project.");
            var member = new ProjectMember
            {
                ProjectId = projectId,
                UserId = request.UserId,
                JoinedAt = DateTime.UtcNow
            };
            _context.ProjectMembers.Add(member);
            await _context.SaveChangesAsync();
            return new ProjectMemberResponse
            {
                Id = member.Id,
                ProjectId = member.ProjectId,
                UserId = member.UserId,
                UserName = user.Name,
                UserEmail = user.Email,
                JoinedAt = member.JoinedAt
            };
        }

        public async Task<IEnumerable<ProjectMemberResponse>> GetMembersAsync(int projectId)
        {
            var projectExists = await _context.Projects.AnyAsync(p => p.Id == projectId);
            
            if (!projectExists)
                throw new InvalidOperationException("Project not found.");

            var members = await _context.ProjectMembers
                .AsNoTracking()
                .Where(pm => pm.ProjectId == projectId)
                .Include(pm => pm.User) // Include user details
                .ToListAsync();

            return members.Select(pm => new ProjectMemberResponse
            {
                Id = pm.Id,
                ProjectId = pm.ProjectId,
                UserId = pm.UserId,
                UserName = pm.User.Name,
                UserEmail = pm.User.Email,
                JoinedAt = pm.JoinedAt
            });
        }

        public async Task<bool> RemoveMemberAsync(int projectId, int userId)
        {
            var member = await _context.ProjectMembers
                .FirstOrDefaultAsync(pm =>
                    pm.ProjectId == projectId &&
                    pm.UserId == userId);
            if(member == null)
                return false;

            _context.ProjectMembers.Remove(member);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
