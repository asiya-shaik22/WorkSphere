using WorkSphere.DTOs.Users;

namespace WorkSphere.Services
{
    public interface IUserService
    { 
        Task<UserResponse> CreateUserAsync(CreateUserRequest request);

        Task<IEnumerable<UserResponse>> GetAllUsersAsync(); 

        Task<UserResponse> GetUserByIdAsync(int id);
        Task<UserResponse> UpdateUserAsync(int id, UpdateUserRequest request);
        Task<bool> DeactivateUserAsync(int id);

    }
}
