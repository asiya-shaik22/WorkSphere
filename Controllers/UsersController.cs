
using Microsoft.AspNetCore.Mvc;
using WorkSphere.DTOs.Users;
using WorkSphere.Services;


namespace WorkSphere.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // POST: api/users
    [HttpPost]
    public async Task<ActionResult<UserResponse>> CreateUser(
        CreateUserRequest request)
    {
        try
        {
            var user = await _userService.CreateUserAsync(request);

            return CreatedAtAction(
                nameof(GetUserById),
                new { id = user.Id },
                user);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // GET: api/users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsers()
    {
        var users = await _userService.GetAllUsersAsync();

        return Ok(users);
    }

    // GET: api/users/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponse>> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
            return NotFound(new { message = "User not found." });

        return Ok(user);
    }

    // PUT: api/users/1
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserResponse>> UpdateUser(
        int id,
        UpdateUserRequest request)
    {
        try
        {
            var user = await _userService.UpdateUserAsync(id, request);

            if (user == null)
                return NotFound(new { message = "User not found." });

            return Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // DELETE: api/users/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeactivateUser(int id)
    {
        var result = await _userService.DeactivateUserAsync(id);

        if (!result)
            return NotFound(new { message = "User not found." });

        return NoContent();
    }
}
