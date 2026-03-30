using TaskManagement.Application.DTOs.Task;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Interfaces;

public interface ITaskService
{
    Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, Guid currentUserId);
    Task<IEnumerable<TaskDto>> GetAllTasksAsync();
    Task<IEnumerable<TaskDto>> GetFilteredTasksAsync(int? status, int? priority, Guid? departmentId, bool orderByDesc);
    Task UpdateStatusAsync(UpdateTaskStatusDto dto, Guid currentUserId);
    Task UpdateTaskAsync(UpdateTaskDto dto, Guid currentUserId);
    Task AssignTaskAsync(AssignTaskDto dto, Guid currentUserId);
    Task DeleteTaskAsync(Guid taskId, Guid currentUserId);
}