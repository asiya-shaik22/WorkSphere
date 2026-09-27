using Microsoft.EntityFrameworkCore;
using WorkSphere.Data;
using WorkSphere.DTOs.Comments;
using WorkSphere.Models;


namespace WorkSphere.Services;
public class CommentService : ICommentService
{
    private readonly AppDbContext _context;
    public CommentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CommentResponse> CreateCommentAsync(int taskId,int userId, CommentRequest request)
    {
        // Validate task existence
        var taskExists = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);
        if (taskExists == null)
            throw new InvalidOperationException("Task not found.");

        // Validate user existence
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new InvalidOperationException("User not found.");

        if(!user.IsActive)
            throw new InvalidOperationException("User is not active.");

        var isMember = await _context.ProjectMembers
            .AnyAsync(pm => pm.ProjectId == taskExists.ProjectId && pm.UserId == userId);

        if (!isMember)
            throw new InvalidOperationException("User is not a member of the project.");

        if (string.IsNullOrWhiteSpace(request.Content))
            throw new ArgumentException("Comment content is required.");

        // Create the comment
        var comment = new Comment
        {
            TaskId = taskId,
            UserId = userId,
            Content = request.Content.Trim(),
            CreatedAt = DateTime.UtcNow
        };
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        return MapToResponse(comment, user);

    }

    public async Task<IEnumerable<CommentResponse>> GetCommentsAsync(int taskId)
    {
        var taskExists = await _context.Tasks.AnyAsync(t => t.Id == taskId);
        if (!taskExists)
            throw new InvalidOperationException("Task not found.");

        var comments = await _context.Comments
            .AsNoTracking()
            .Where(c => c.TaskId == taskId)
            .Include(c => c.User) // Include user details
            .ToListAsync();
        return comments.Select(c => MapToResponse(c, c.User));
    }

    public async Task<CommentResponse?> UpdateCommentAsync(int id, int userId, CommentRequest request)
    {
        var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
        if (comment == null)
            return null;
        if (comment.UserId != userId)
            throw new InvalidOperationException("User is not authorized to update this comment.");
        if (string.IsNullOrWhiteSpace(request.Content))
            throw new ArgumentException("Comment content is required.");
        comment.Content = request.Content.Trim();
        comment.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        return MapToResponse(comment, user!);
    }

    public async Task<bool> DeleteCommentAsync(int id, int userId)
    {
        var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
        if (comment == null)
            return false;
        if (comment.UserId != userId)
            throw new InvalidOperationException("User is not authorized to delete this comment.");
        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
        return true;
    }

    private static CommentResponse MapToResponse(Comment comment, User user)
    {
        return new CommentResponse
        {
            Id = comment.Id,
            TaskId = comment.TaskId,
            UserId = comment.UserId,
            UserName = user.Name,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };
    }

}

