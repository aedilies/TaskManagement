using Moq;
using TaskManagement.Application.DTOs.Task;
using TaskManagement.Application.Services;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Repositories;
using Xunit;

public class TaskServiceTests
{
    private readonly Mock<IRepository<TaskItem>> _taskRepository;
    private readonly Mock<IRepository<User>> _userRepository;
    private readonly TaskService _service;

    public TaskServiceTests()
    {
        _taskRepository = new Mock<IRepository<TaskItem>>();
        _userRepository = new Mock<IRepository<User>>();
        _service = new TaskService(_taskRepository.Object, _userRepository.Object);
    }

    [Fact]
    public async Task CreateTask_Should_Create_Task()
    {
        // Arrange
        var assignedUserId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid();

        var dto = new CreateTaskDto
        {
            Title = "Test task",
            AssignedToUserId = assignedUserId,
            Priority = 1
        };

        _userRepository.Setup(x => x.GetByIdAsync(assignedUserId, default))
            .ReturnsAsync(new User { Id = assignedUserId, FullName = "Assigned User" });

        _taskRepository.Setup(x => x.AddAsync(It.IsAny<TaskItem>(), default))
            .Returns(Task.CompletedTask);
        _taskRepository.Setup(x => x.SaveChangesAsync(default))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateTaskAsync(dto, currentUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Title, result.Title);
        Assert.Equal("Assigned User", result.AssignedTo);

        _taskRepository.Verify(x => x.AddAsync(It.IsAny<TaskItem>(), default), Times.Once);
        _taskRepository.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }
}