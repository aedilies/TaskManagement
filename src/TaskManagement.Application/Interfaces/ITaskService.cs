using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.DTOs.Task;

namespace TaskManagement.Application.Interfaces;

public interface ITaskService
{
    Task<TaskDto> CreateTaskAsync(
        CreateTaskDto dto,
        Guid currentUserId);

    Task<IEnumerable<TaskDto>> GetAllTasksAsync();

    Task UpdateStatusAsync(
        UpdateTaskStatusDto dto,
        Guid currentUserId);

    Task DeleteTaskAsync(
        Guid taskId,
        Guid currentUserId);
}
