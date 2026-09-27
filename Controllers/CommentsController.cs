using Microsoft.AspNetCore.Mvc;
using WorkSphere.DTOs.Comments;
using WorkSphere.Services;


namespace WorkSphere.Controllers
{
    [ApiController]
    [Route("api/tasks/{taskId:int}/comments")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;
        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost]
        public async Task<ActionResult<CommentResponse>> CreateComment(int taskId, int userId, [FromBody] CommentRequest request)
        {
            try
            {
                var comment = await _commentService.CreateCommentAsync(taskId, userId, request);
                return Ok(comment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentResponse>>> GetComments(int taskId)
        {
            try
            {
                var comments = await _commentService.GetCommentsAsync(taskId);
                return Ok(comments);

            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }

        }

        [HttpPut("{commentId:int}")]
        public async Task<ActionResult<CommentResponse>> UpdateComment(int commentId, int userId, [FromBody] CommentRequest request)
        {
            try
            {
                var comment = await _commentService.UpdateCommentAsync(commentId, userId, request);
                if (comment == null)
                {
                    return NotFound(new { message = "Comment not found." });
                }

                return Ok(comment);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();

            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }    
        }

        [HttpDelete("{commentId:int}")]
        public async Task<IActionResult> DeleteComment(int taskId, int commentId, int userId)
        {
            try
            {
                var deleted = await _commentService.DeleteCommentAsync(commentId, userId);
                if (!deleted)
                {
                    return NotFound(new { message = "Comment not found." });
                }
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }


    }
}
