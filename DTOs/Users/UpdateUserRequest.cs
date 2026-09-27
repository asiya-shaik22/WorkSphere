using WorkSphere.Enums;

namespace WorkSphere.DTOs.Users
{
    public class UpdateUserRequest
    {

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public UserRole Role { get; set; }

        public bool IsActive { get; set; } 



    }
}