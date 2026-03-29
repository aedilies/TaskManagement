using Moq;
using TaskManagement.Application.DTOs.Task;
using TaskManagement.Application.Services;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Repositories;

public class TaskServiceTests
{
    private readonly Mock<IRepository<TaskItem>> _taskRepository;
    private readonly Mock<IRepository<User>> _userRepository;

    private readonly TaskService _service;

    public TaskServiceTests()
    {
        _taskRepository = new Mock<IRepository<TaskItem>>();
        _userRepository = new Mock<IRepository<User>>();

        _service = new TaskService(
            _taskRepository.Object,
            _userRepository.Object);
    }

    [Fact]
    public async Task CreateTask_Should_Create_Task()
    {
        var dto = new CreateTaskDto
        {
            Title = "Test task",
            AssignedToUserId = Guid.NewGuid(),
            Priority = 1
        };

        var result = await _service.CreateTaskAsync(
            dto,
            Guid.NewGuid());

        Assert.NotNull(result);
        Assert.Equal("Test task", result.Title);
    }
}