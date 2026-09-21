using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkSphere.Data;
using WorkSphere.Models;

namespace WorkSphere.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/users
        // Create a new user
        [HttpPost]
        public async Task<IActionResult> CreateUser(UserCreateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("Name is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest("Email is required.");
            }

            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == request.Email);

            if (emailExists)
            {
                return Conflict("A user with this email already exists.");
            }

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetUserById),
                new { id = user.Id },
                new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.IsActive,
                    user.CreatedAt
                });
        }

        // GET: api/users
        // Get all users
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.IsActive,
                    u.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/users/{id}
        // Get a specific user
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.IsActive,
                    u.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user);
        }

        // PUT: api/users/{id}
        // Update user
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(
            int id,
            UserUpdateRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("Name is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest("Email is required.");
            }

            var emailExists = await _context.Users
                .AnyAsync(u =>
                    u.Email == request.Email &&
                    u.Id != id);

            if (emailExists)
            {
                return Conflict("A user with this email already exists.");
            }

            user.Name = request.Name;
            user.Email = request.Email;
            user.IsActive = request.IsActive;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                user.Id,
                user.Name,
                user.Email,
                user.IsActive,
                user.CreatedAt
            });
        }
    }

    // Request model for creating a user
    public class UserCreateRequest
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }

    // Request model for updating a user
    public class UserUpdateRequest
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}