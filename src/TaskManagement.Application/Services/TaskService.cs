using TaskManagement.Application.DTOs.Task;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Infrastructure.Repositories;

namespace TaskManagement.Application.Services;

public class TaskService : ITaskService
{
    private readonly IRepository<TaskItem> _taskRepository;
    private readonly IRepository<User> _userRepository;

    public TaskService(
        IRepository<TaskItem> taskRepository,
        IRepository<User> userRepository)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
    }

    public async Task<TaskDto> CreateTaskAsync(
        CreateTaskDto dto,
        Guid currentUserId)
    {
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            Priority = (TaskPriority)dto.Priority,
            Status = TaskItemStatus.New,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = currentUserId,
            AssignedToUserId = dto.AssignedToUserId
        };

        await _taskRepository.AddAsync(task);
        await _taskRepository.SaveChangesAsync();

        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status.ToString(),
            Priority = task.Priority.ToString(),
            CreatedAt = task.CreatedAt
        };
    }

    public async Task<IEnumerable<TaskDto>> GetAllTasksAsync()
    {
        var tasks = await _taskRepository.GetAllAsync();

        return tasks.Select(t => new TaskDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Status = t.Status.ToString(),
            Priority = t.Priority.ToString(),
            CreatedAt = t.CreatedAt
        });
    }

    public async Task UpdateStatusAsync(
        UpdateTaskStatusDto dto,
        Guid currentUserId)
    {
        var task = await _taskRepository.GetByIdAsync(dto.TaskId);

        if (task == null)
            throw new Exception("Task not found");

        task.Status = (TaskItemStatus)dto.Status;
        task.UpdatedAt = DateTime.UtcNow;

        _taskRepository.Update(task);
        await _taskRepository.SaveChangesAsync();
    }

    public async Task DeleteTaskAsync(
        Guid taskId,
        Guid currentUserId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);

        if (task == null)
            throw new Exception("Task not found");

        _taskRepository.Delete(task);

        await _taskRepository.SaveChangesAsync();
    }
}