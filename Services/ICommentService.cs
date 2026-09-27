using WorkSphere.DTOs.Comments;

namespace WorkSphere.Services;

public interface ICommentService
{
    Task<CommentResponse> CreateCommentAsync(int taskId, int userId, CommentRequest request);

    Task<IEnumerable<CommentResponse>> GetCommentsAsync(int taskId);

    Task<CommentResponse?> UpdateCommentAsync(int id, int userId, CommentRequest request);

    Task<bool> DeleteCommentAsync(int id, int userId);

}

