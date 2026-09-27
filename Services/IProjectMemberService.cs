using WorkSphere.DTOs.ProjectMembers;

namespace WorkSphere.Services
{
    public interface IProjectMemberService
    {
        Task<ProjectMemberResponse> AddMemberAsync(int projectId, AddProjectMemberRequest request);

        Task<IEnumerable<ProjectMemberResponse>> GetMembersAsync(int projectId);

        Task<bool> RemoveMemberAsync(int projectId, int userId);
    }
}
