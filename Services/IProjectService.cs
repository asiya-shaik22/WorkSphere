using WorkSphere.DTOs.Projects;


namespace WorkSphere.Services
{
    public interface IProjectService
    {
        Task<ProjectResponse> CreateProjectAsync(CreateProjectRequest request);
        Task<IEnumerable<ProjectResponse>> GetProjectsAsync();
        Task<ProjectResponse?> GetProjectByIdAsync(int id);
        Task<ProjectResponse?> UpdateProjectAsync(int id, UpdateProjectRequest request);
        Task<bool> DeleteProjectAsync(int id);

    }
}
