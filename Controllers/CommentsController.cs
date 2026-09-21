using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkSphere.Data;
using WorkSphere.Models;

namespace WorkSphere.Controllers
{
    [ApiController]
    [Route("api/tasks/{taskId}/comments")]
    public class CommentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CommentsController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/tasks/{taskId}/comments
        [HttpPost]
        public async Task<IActionResult> AddComment(
            int taskId,
            CreateCommentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
                return BadRequest("Comment content is required.");

            var taskExists = await _context.Tasks
                .AnyAsync(t => t.Id == taskId);

            if (!taskExists)
                return NotFound("Task not found.");

            var userExists = await _context.Users
                .AnyAsync(u => u.Id == request.UserId);

            if (!userExists)
                return BadRequest("User not found.");

            var comment = new Comment
            {
                Content = request.Content,
                TaskId = taskId,
                UserId = request.UserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetComments),
                new { taskId },
                comment);
        }

        // GET: api/tasks/{taskId}/comments
        [HttpGet]
        public async Task<IActionResult> GetComments(int taskId)
        {
            var taskExists = await _context.Tasks
                .AnyAsync(t => t.Id == taskId);

            if (!taskExists)
                return NotFound("Task not found.");

            var comments = await _context.Comments
                .Where(c => c.TaskId == taskId)
                .Include(c => c.User)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new
                {
                    c.Id,
                    c.Content,
                    c.TaskId,
                    c.UserId,
                    UserName = c.User.Name,
                    c.CreatedAt,
                    c.UpdatedAt
                })
                .ToListAsync();

            return Ok(comments);
        }

        // PUT: api/tasks/{taskId}/comments/{commentId}
        [HttpPut("{commentId}")]
        public async Task<IActionResult> UpdateComment(
            int taskId,
            int commentId,
            UpdateCommentRequest request)
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c =>
                    c.Id == commentId &&
                    c.TaskId == taskId);

            if (comment == null)
                return NotFound("Comment not found.");

            if (string.IsNullOrWhiteSpace(request.Content))
                return BadRequest("Comment content is required.");

            comment.Content = request.Content;
            comment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(comment);
        }

        // DELETE: api/tasks/{taskId}/comments/{commentId}
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(
            int taskId,
            int commentId)
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c =>
                    c.Id == commentId &&
                    c.TaskId == taskId);

            if (comment == null)
                return NotFound("Comment not found.");

            _context.Comments.Remove(comment);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Comment deleted successfully."
            });
        }
    }

    public class CreateCommentRequest
    {
        public string Content { get; set; } = string.Empty;

        public int UserId { get; set; }
    }

    public class UpdateCommentRequest
    {
        public string Content { get; set; } = string.Empty;
    }
}