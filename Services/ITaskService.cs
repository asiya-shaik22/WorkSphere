using WorkSphere.DTOs.Tasks;

namespace WorkSphere.Services;


public interface ITaskService
{
    Task<TaskResponse> CreateTaskAsync(CreateTaskRequest request);
    Task<TaskResponse?> GetTaskByIdAsync(int taskId);
    Task<IEnumerable<TaskResponse>> GetTasksAsync(int projectId);

    Task<TaskResponse?> UpdateTaskAsync(int taskId, UpdateTaskRequest request);
    Task<bool> DeleteTaskAsync(int taskId);
    

}
