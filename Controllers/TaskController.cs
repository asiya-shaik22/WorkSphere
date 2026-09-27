using Microsoft.AspNetCore.Mvc;
using WorkSphere.DTOs.Tasks;
using WorkSphere.Services;

namespace WorkSphere.API.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponse>> CreateTask(
        CreateTaskRequest request)
    {
        try
        {
            var task = await _taskService.CreateTaskAsync(request);

            return Ok(task);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("project/{projectId:int}")]
    public async Task<ActionResult<IEnumerable<TaskResponse>>> GetTasks(
        int projectId)
    {
        var tasks = await _taskService.GetTasksAsync(projectId);

        return Ok(tasks);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskResponse>> GetTask(int id)
    {
        var task = await _taskService.GetTaskByIdAsync(id);

        if (task == null)
            return NotFound(new { message = "Task not found." });

        return Ok(task);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TaskResponse>> UpdateTask(
        int id,
        UpdateTaskRequest request)
    {
        try
        {
            var task =
                await _taskService.UpdateTaskAsync(id, request);

            if (task == null)
                return NotFound(new { message = "Task not found." });

            return Ok(task);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var deleted = await _taskService.DeleteTaskAsync(id);

        if (!deleted)
            return NotFound(new { message = "Task not found." });

        return NoContent();
    }
}