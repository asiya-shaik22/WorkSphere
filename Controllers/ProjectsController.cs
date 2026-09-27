using Microsoft.AspNetCore.Mvc;
using WorkSphere.DTOs.Projects;
namespace WorkSphere.Services;

[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    // POST: api/projects
    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request)
    {
        try
        {
            var project = await _projectService.CreateProjectAsync(request);
            return CreatedAtAction(nameof(GetProjectById), new { id = project.Id }, project);

        }
        catch (Exception ex) when (
                ex is ArgumentException ||
                ex is InvalidOperationException)
            {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    // GET: api/projects
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectResponse>>> GetProjects()
    {
        var projects = await _projectService.GetProjectsAsync();
        return Ok(projects);
    }

    // GET: api/projects/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectResponse>> GetProjectById(int id)
    {
        var project = await _projectService.GetProjectByIdAsync(id);
        if (project == null)
        {
            return NotFound( new {message= $"Project with ID {id} not found." });
        }
        return Ok(project);
    }

    // PUT: api/projects/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProjectResponse>> UpdateProject(
    int id,
    [FromBody] UpdateProjectRequest request)
    {
        try
        {
            var updatedProject =
                await _projectService.UpdateProjectAsync(id, request);

            if (updatedProject == null)
            {
                return NotFound(new
                {
                    message = $"Project with ID {id} not found."
                });
            }

            return Ok(updatedProject);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }


    // DELETE: api/projects/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var result = await _projectService.DeleteProjectAsync(id);
        if (!result)
        {
            return NotFound(new { message = $"Project with ID {id} not found." });
        }
        return NoContent();
    }
}
