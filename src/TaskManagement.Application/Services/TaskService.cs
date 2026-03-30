using Microsoft.EntityFrameworkCore;
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

    public TaskService(IRepository<TaskItem> taskRepository, IRepository<User> userRepository)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
    }

    public async Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, Guid currentUserId)
    {
        // Загружаем текущего пользователя с ролью
        var currentUser = await _userRepository.GetAllQueryable()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == currentUserId);
        if (currentUser == null)
            throw new Exception("Current user not found");

        if (currentUser.Role?.Code != "BOSS")
            throw new UnauthorizedAccessException("Only BOSS can create tasks");

        var assignedUser = await _userRepository.GetByIdAsync(dto.AssignedToUserId);
        if (assignedUser == null)
            throw new Exception("Assigned user not found");

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
            CreatedAt = task.CreatedAt,
            CreatedBy = currentUser.FullName,
            AssignedTo = assignedUser.FullName
        };
    }

    public async Task<IEnumerable<TaskDto>> GetAllTasksAsync()
    {
        var tasks = await _taskRepository.GetAllQueryable()
            .Include(t => t.CreatedByUser)
            .Include(t => t.AssignedToUser)
            .ToListAsync();

        return tasks.Select(MapToDto);
    }

    public async Task<IEnumerable<TaskDto>> GetFilteredTasksAsync(int? status, int? priority, Guid? departmentId, bool orderByDesc)
    {
        IQueryable<TaskItem> query = _taskRepository.GetAllQueryable()
            .Include(t => t.AssignedToUser)
            .Include(t => t.CreatedByUser);

        if (status.HasValue)
            query = query.Where(t => (int)t.Status == status.Value);

        if (priority.HasValue)
            query = query.Where(t => (int)t.Priority == priority.Value);

        if (departmentId.HasValue)
            query = query.Where(t => t.AssignedToUser!.DepartmentId == departmentId.Value);

        query = orderByDesc ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt);

        var list = await query.ToListAsync();
        return list.Select(MapToDto);
    }

    public async Task UpdateStatusAsync(UpdateTaskStatusDto dto, Guid currentUserId)
    {
        var task = await _taskRepository.GetByIdAsync(dto.TaskId);
        if (task == null) throw new Exception("Task not found");

        var currentUser = await _userRepository.GetAllQueryable()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == currentUserId);
        if (currentUser == null) throw new Exception("Current user not found");

        if (currentUser.Role?.Code == "OBSERVER")
            throw new UnauthorizedAccessException("Observer cannot modify tasks");

        if (currentUser.Role?.Code == "EMPLOYEE" && task.AssignedToUserId != currentUserId)
            throw new UnauthorizedAccessException("Employee can only modify own tasks");

        task.Status = (TaskItemStatus)dto.Status;
        task.UpdatedAt = DateTime.UtcNow;

        _taskRepository.Update(task);
        await _taskRepository.SaveChangesAsync();
    }

    public async Task UpdateTaskAsync(UpdateTaskDto dto, Guid currentUserId)
    {
        var task = await _taskRepository.GetByIdAsync(dto.TaskId);
        if (task == null) throw new Exception("Task not found");

        var currentUser = await _userRepository.GetAllQueryable()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == currentUserId);
        if (currentUser == null) throw new Exception("Current user not found");

        if (currentUser.Role?.Code == "OBSERVER")
            throw new UnauthorizedAccessException("Observer cannot modify tasks");

        if (currentUser.Role?.Code == "EMPLOYEE" && task.AssignedToUserId != currentUserId)
            throw new UnauthorizedAccessException("Employee can only edit own tasks");

        if (!string.IsNullOrEmpty(dto.Title)) task.Title = dto.Title;
        if (!string.IsNullOrEmpty(dto.Description)) task.Description = dto.Description;
        if (dto.Priority.HasValue) task.Priority = (TaskPriority)dto.Priority.Value;

        task.UpdatedAt = DateTime.UtcNow;

        _taskRepository.Update(task);
        await _taskRepository.SaveChangesAsync();
    }

    public async Task AssignTaskAsync(AssignTaskDto dto, Guid currentUserId)
    {
        var task = await _taskRepository.GetByIdAsync(dto.TaskId);
        if (task == null) throw new Exception("Task not found");

        var currentUser = await _userRepository.GetAllQueryable()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == currentUserId);
        if (currentUser == null) throw new Exception("Current user not found");

        if (currentUser.Role?.Code != "BOSS")
            throw new UnauthorizedAccessException("Only BOSS can assign tasks");

        var assignedUser = await _userRepository.GetByIdAsync(dto.AssignedToUserId);
        if (assignedUser == null) throw new Exception("Assigned user not found");

        task.AssignedToUserId = dto.AssignedToUserId;
        task.UpdatedAt = DateTime.UtcNow;

        _taskRepository.Update(task);
        await _taskRepository.SaveChangesAsync();
    }

    public async Task DeleteTaskAsync(Guid taskId, Guid currentUserId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null) throw new Exception("Task not found");

        var currentUser = await _userRepository.GetAllQueryable()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == currentUserId);
        if (currentUser == null) throw new Exception("Current user not found");

        if (currentUser.Role?.Code == "OBSERVER")
            throw new UnauthorizedAccessException("Observer cannot delete tasks");

        if (currentUser.Role?.Code == "EMPLOYEE" && task.AssignedToUserId != currentUserId)
            throw new UnauthorizedAccessException("Employee can only delete own tasks");

        _taskRepository.Delete(task);
        await _taskRepository.SaveChangesAsync();
    }

    private TaskDto MapToDto(TaskItem t)
    {
        return new TaskDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Status = t.Status.ToString(),
            Priority = t.Priority.ToString(),
            CreatedAt = t.CreatedAt,
            CreatedBy = t.CreatedByUser?.FullName ?? "",
            AssignedTo = t.AssignedToUser?.FullName ?? ""
        };
    }
}