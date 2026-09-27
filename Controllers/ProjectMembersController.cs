using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkSphere.Data;
using WorkSphere.Models;
using WorkSphere.Services;
using WorkSphere.DTOs.ProjectMembers;


namespace WorkSphere.Controllers;

[ApiController]
[Route("api/projects/{projectId:int}/members")]
public class ProjectMembersController : ControllerBase
{
    private readonly IProjectMemberService _memberService;
    public ProjectMembersController(IProjectMemberService memberService)
    {
        _memberService = memberService;
    }

    // POST: api/projects/{projectId}/members
    [HttpPost]
    public async Task<IActionResult> AddMember(int projectId, [FromBody] AddProjectMemberRequest request)
    {
        try
        {
            var member = await _memberService.AddMemberAsync(projectId, request);
            return Ok(member);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]   
    public async Task<ActionResult<IEnumerable<ProjectMemberResponse>>> GetMembers(int projectId)
    {
        try
        {
            var members = await _memberService.GetMembersAsync(projectId);
            return Ok(members);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }


    [HttpDelete("{userId:int}")]
    public async Task<IActionResult> RemoveMember(int projectId, int userId)
    {
        var removed = await _memberService.RemoveMemberAsync(projectId, userId);
        if (!removed)
            return NotFound(new { message = "Project Member not found." });
        return NoContent();
    }

}