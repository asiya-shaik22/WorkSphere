using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkSphere.Data;
using WorkSphere.Models;

namespace WorkSphere.Controllers
{
    [ApiController]
    [Route("api/projects/{projectId}/members")]
    public class ProjectMembersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectMembersController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/projects/{projectId}/members
        [HttpPost]
        public async Task<IActionResult> AddMember(
            int projectId,
            AddProjectMemberRequest request)
        {
            var projectExists = await _context.Projects
                .AnyAsync(p => p.Id == projectId);

            if (!projectExists)
                return NotFound("Project not found.");

            var userExists = await _context.Users
                .AnyAsync(u => u.Id == request.UserId);

            if (!userExists)
                return NotFound("User not found.");

            var alreadyMember = await _context.ProjectMembers
                .AnyAsync(pm =>
                    pm.ProjectId == projectId &&
                    pm.UserId == request.UserId);

            if (alreadyMember)
                return Conflict("User is already a member of this project.");

            var member = new ProjectMember
            {
                ProjectId = projectId,
                UserId = request.UserId,
                JoinedAt = DateTime.UtcNow
            };

            _context.ProjectMembers.Add(member);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetMembers),
                new { projectId },
                new
                {
                    member.Id,
                    member.ProjectId,
                    member.UserId,
                    member.JoinedAt
                });
        }

        // GET: api/projects/{projectId}/members
        [HttpGet]
        public async Task<IActionResult> GetMembers(int projectId)
        {
            var projectExists = await _context.Projects
                .AnyAsync(p => p.Id == projectId);

            if (!projectExists)
                return NotFound("Project not found.");

            var members = await _context.ProjectMembers
                .Where(pm => pm.ProjectId == projectId)
                .Include(pm => pm.User)
                .Select(pm => new
                {
                    pm.User.Id,
                    pm.User.Name,
                    pm.User.Email,
                    pm.User.IsActive,
                    pm.JoinedAt
                })
                .ToListAsync();

            return Ok(members);
        }

        // DELETE:
        // api/projects/{projectId}/members/{userId}
        [HttpDelete("{userId}")]
        public async Task<IActionResult> RemoveMember(
            int projectId,
            int userId)
        {
            var member = await _context.ProjectMembers
                .FirstOrDefaultAsync(pm =>
                    pm.ProjectId == projectId &&
                    pm.UserId == userId);

            if (member == null)
                return NotFound("Project member not found.");

            _context.ProjectMembers.Remove(member);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Project member removed successfully."
            });
        }
    }

    public class AddProjectMemberRequest
    {
        public int UserId { get; set; }
    }
}