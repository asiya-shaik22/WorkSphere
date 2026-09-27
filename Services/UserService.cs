using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkSphere.Data;
using WorkSphere.DTOs.Users;
using WorkSphere.Models;
using WorkSphere.Services;

namespace WorkSphere.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher;

    public UserService(AppDbContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task<UserResponse> CreateUserAsync(
        CreateUserRequest request)
    {
        // Check duplicate email
        bool emailExists = await _context.Users
            .AnyAsync(u => u.Email == request.Email);

        if (emailExists)
            throw new InvalidOperationException("Email already exists.");

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Role = request.Role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Hash password
        user.PasswordHash =
            _passwordHasher.HashPassword(user, request.Password);

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return MapToResponse(user);
    }

    public async Task<IEnumerable<UserResponse>> GetUsersAsync()
    {
        var users = await _context.Users
            .AsNoTracking()
            .ToListAsync();

        return users.Select(MapToResponse);
    }

    public Task<IEnumerable<UserResponse>> GetAllUsersAsync()
    {
        return GetUsersAsync();
    }
                
    public async Task<UserResponse> GetUserByIdAsync(int id)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            throw new KeyNotFoundException($"User with id {id} not found.");

        return MapToResponse(user);
    }

    public async Task<UserResponse> UpdateUserAsync(
        int id,
        UpdateUserRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            throw new KeyNotFoundException($"User with id {id} not found.");

        bool emailExists = await _context.Users
            .AnyAsync(u => u.Email == request.Email && u.Id != id);

        if (emailExists)
            throw new InvalidOperationException("Email already exists.");

        user.Name = request.Name;
        user.Email = request.Email;
        user.Role = request.Role;
        user.IsActive = request.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToResponse(user);
    }

    public async Task<bool> DeactivateUserAsync(int id)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return false;

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    private static UserResponse MapToResponse(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}


